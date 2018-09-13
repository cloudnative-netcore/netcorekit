using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetCoreKit.Domain;
using NetCoreKit.Infrastructure.Mappers;
using Newtonsoft.Json;

namespace NetCoreKit.Infrastructure.Bus.Kafka
{
  /// <summary>
  /// Source: https://github.com/ivanpaulovich/event-sourcing-jambo
  /// Notes: don't upgrade Confluent.Kafka to 0.11.5, there is a bug that doesn't fire out any event 
  /// </summary>
  public class EventBus : IEventBus
  {
    private readonly string _brokerList;

    private readonly ILogger<EventBus> _logger;

    private readonly Producer<string, string> _producer;
    private readonly IServiceProvider _serviceProvider;

    public EventBus(IServiceProvider serviceProvider, IOptions<KafkaOptions> options, ILoggerFactory factory)
    {
      _serviceProvider = serviceProvider;

      _brokerList = options.Value.Brokers;
      _producer = new Producer<string, string>(
        new Dictionary<string, object> {["bootstrap.servers"] = _brokerList},
        new StringSerializer(Encoding.UTF8), new StringSerializer(Encoding.UTF8));

      _producer.Flush(TimeSpan.FromSeconds(10));

      _logger = factory.CreateLogger<EventBus>();
    }

    public async Task Publish(IEvent @event, params string[] topics)
    {
      if (topics.Length <= 0) throw new CoreException("Publish - Topic to publish should be at least one.");

      var data = JsonConvert.SerializeObject(@event, Formatting.Indented);
      foreach (var topic in topics)
      {
        var result = _producer.ProduceAsync(topic, @event.GetType().AssemblyQualifiedName, data);
        await Task.WhenAll(
          result.ContinueWith(task =>
          {
            if (task.Result.Error.HasError)
              _logger.LogInformation($"Publish - IS ERROR RESULT {result.Result.Error.Reason}");
            else
              _logger.LogInformation("Publish - Delivered {0}\nPartition: {0}, Offset: {1}", task.Result.Value,
                task.Result.Partition, task.Result.Offset);

            if (task.IsFaulted)
              _logger.LogInformation("Publish - IS FAULTED");

            if (task.Exception != null)
              _logger.LogInformation(result.Exception?.Message);

            if (task.IsCanceled)
              _logger.LogInformation("Publish - IS CANCELLED");
          }));

        _logger.LogTrace($"Publish - Events are writted to Kafka. Topic name: {topic}.");
      }
    }

    public async Task Subscribe(params string[] topics)
    {
      if (topics.Length <= 0)
        throw new CoreException("Subscribe - Topics to subscribe should be at least one.");

      using (var consumer = new Consumer<string, string>(
        ConstructConfig(_brokerList, true),
        new StringDeserializer(Encoding.UTF8),
        new StringDeserializer(Encoding.UTF8)))
      {
        consumer.OnPartitionEOF += (_, end)
          => _logger.LogInformation(
            $"Subscribe - Reached end of topic {end.Topic} partition {end.Partition}, next message will be at offset {end.Offset}");

        consumer.OnError += (_, error)
          => _logger.LogError($"Subscribe - Error: {error}");

        consumer.OnConsumeError += (_, msg)
          => _logger.LogError(
            $"Subscribe - Error consuming from topic/partition/offset {msg.Topic}/{msg.Partition}/{msg.Offset}: {msg.Error}");

        consumer.OnOffsetsCommitted += (_, commit) =>
        {
          _logger.LogInformation($"Subscribe - [{string.Join(", ", commit.Offsets)}]");

          if (commit.Error)
            _logger.LogError($"Subscribe- Failed to commit offsets: {commit.Error}");
          _logger.LogInformation($"Subscribe - Successfully committed offsets: [{string.Join(", ", commit.Offsets)}]");
        };

        consumer.OnPartitionsAssigned += (_, partitions) =>
        {
          _logger.LogInformation(
            $"Subscribe - Assigned partitions: [{string.Join(", ", partitions)}], member id: {consumer.MemberId}");
          consumer.Assign(partitions);
        };

        consumer.OnPartitionsRevoked += (_, partitions) =>
        {
          _logger.LogInformation($"Subscribe - Revoked partitions: [{string.Join(", ", partitions)}]");
          consumer.Unassign();
        };

        consumer.OnStatistics += (_, json)
          => _logger.LogInformation($"Subscribe - Statistics: {json}");

        consumer.Subscribe(topics);

        _logger.LogInformation($"Subscribe - Subscribed to: [{string.Join(", ", consumer.Subscription)}]");

        var cancelled = false;
        Console.CancelKeyPress += (_, e) =>
        {
          e.Cancel = true; // prevent the process from terminating.
          cancelled = true;
        };

        _logger.LogInformation("Subscribe - Ctrl-C to exit.");
        while (!cancelled)
        {
          if (!consumer.Consume(out var msg, TimeSpan.FromSeconds(1))) continue;
          _logger.LogInformation($"Subscribe - Topic: {msg.Topic} Partition: {msg.Partition} Offset: {msg.Offset} {msg.Value}");

          var eventType = Type.GetType(msg.Key);
          if (eventType == null) continue;

          var @event = (IEvent)JsonConvert.DeserializeObject(msg.Value, eventType);
          using (var scope = _serviceProvider.CreateScope())
          {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var noti = @event.MapTo<IEvent, INotification>();
            await mediator.Publish(noti);
          }
        }
      }
    }

    public async Task SubscribeAsync(params string[] topics)
    {
      if (topics.Length <= 0)
        throw new CoreException("SubscribeAsync - Topics to subscribe should be at least one.");

      using (var consumer = new Consumer<string, string>(
        ConstructConfig(_brokerList, true),
        new StringDeserializer(Encoding.UTF8),
        new StringDeserializer(Encoding.UTF8)))
      {
        consumer.OnMessage += async (o, e) =>
        {
          _logger.LogInformation($"SubscribeAsync - Topic: {e.Topic} Partition: {e.Partition} Offset: {e.Offset} {e.Value}");

          var eventType = Type.GetType(e.Key);
          if (eventType == null)
            return;

          var domainEvent = (INotification)JsonConvert.DeserializeObject(e.Value, eventType);
          using (var scope = _serviceProvider.CreateScope())
          {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            await mediator.Publish(domainEvent);
          }
        };

        consumer.OnError += (_, e)
          => _logger.LogError("SubscribeAsync - Error: " + e.Reason);

        consumer.OnConsumeError += (_, e)
          => _logger.LogError("SubscribeAsync - Consume error: " + e.Error.Reason);

        consumer.Subscribe(topics);

        var cts = new CancellationTokenSource();
        var consumeTask = Task.Factory.StartNew(() =>
        {
          while (!cts.Token.IsCancellationRequested) consumer.Poll(TimeSpan.FromSeconds(1));
        }, cts.Token);

        consumeTask.Wait(cts.Token);
      }

      await Task.FromResult(true);
    }

    public void Dispose()
    {
      _producer?.Dispose();
    }

    private static IDictionary<string, object> ConstructConfig(string brokerList, bool enableAutoCommit)
    {
      return new Dictionary<string, object>
      {
        ["group.id"] = "netcorekit-consumer",
        ["bootstrap.servers"] = brokerList,
        ["enable.auto.commit"] = enableAutoCommit,
        ["auto.commit.interval.ms"] = 5000,
        ["statistics.interval.ms"] = 60000,
        //["debug"] = "all",
        ["default.topic.config"] = new Dictionary<string, object>
        {
          ["auto.offset.reset"] = "latest"
        }
      };
    }
  }
}
