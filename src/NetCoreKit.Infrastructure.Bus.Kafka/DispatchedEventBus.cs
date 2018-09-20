using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Confluent.Kafka;
using Google.Protobuf;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetCoreKit.Domain;
using NetCoreKit.Infrastructure.Mappers;
using Null = Confluent.Kafka.Null;

namespace NetCoreKit.Infrastructure.Bus.Kafka
{
  /// <summary>
  ///   Source: https://github.com/ivanpaulovich/event-sourcing-jambo
  ///   Notes: don't upgrade Confluent.Kafka to 0.11.5, there is a bug that doesn't fire out any event
  /// </summary>
  public class DispatchedEventBus : IDispatchedEventBus
  {
    private readonly string _brokerList;

    private readonly ILogger<DispatchedEventBus> _logger;

    private readonly IServiceProvider _serviceProvider;

    public DispatchedEventBus(IServiceProvider serviceProvider, IOptions<KafkaOptions> options, ILoggerFactory factory)
    {
      _serviceProvider = serviceProvider;
      _brokerList = options.Value.Brokers;
      _logger = factory.CreateLogger<DispatchedEventBus>();
    }

    public async Task Publish<TMessage>(TMessage @event, params string[] topics)
      where TMessage : IMessage<TMessage>
    {
      if (topics.Length <= 0) throw new CoreException("Publish - Topic to publish should be at least one.");

      using (var producer = new Producer<Null, TMessage>(
        new Dictionary<string, object> {["bootstrap.servers"] = _brokerList},
        null,
        new ProtoSerializer<TMessage>()))
      {
        foreach (var topic in topics)
        {
          var result = producer.ProduceAsync(topic, null, @event);
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

          producer.Flush(TimeSpan.FromSeconds(10));
        }
      }
    }

    public async Task Subscribe<TMessage>(params string[] topics)
      where TMessage : IMessage<TMessage>, new()
    {
      if (topics.Length <= 0)
        throw new CoreException("Subscribe - Topics to subscribe should be at least one.");

      using (var consumer = new Consumer<Null, TMessage>(
        ConstructConfig(_brokerList, true),
        null,
        new ProtoDeserializer<TMessage>()))
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
          _logger.LogInformation(
            $"Subscribe - Topic: {msg.Topic} Partition: {msg.Partition} Offset: {msg.Offset} {msg.Value}");

          if (msg.Value == null) continue;

          using (var scope = _serviceProvider.CreateScope())
          {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var noti = msg.Value.MapTo<TMessage, INotification>();

            var msgField = msg.Value.Descriptor.FindFieldByName("Key");
            var msgValue = msgField.Accessor.GetValue(msg.Value);

            var currentAssembly = Assembly.GetExecutingAssembly();
            var callerAssemblies = new StackTrace().GetFrames()
              .Select(x => x.GetMethod().ReflectedType.Assembly).Distinct()
              .Where(x => x.GetReferencedAssemblies().Any(y => y.FullName == currentAssembly.FullName));

            var initialAssembly = callerAssemblies.Last();

            var notific = initialAssembly.DefinedTypes
              .FirstOrDefault(x => x.AssemblyQualifiedName.Contains(msgValue.ToString()));

            var obj = (INotification)Activator.CreateInstance(notific);

            var abc = Mapper.Map(msg.Value, obj);

            await mediator.Publish(abc);
          }
        }
      }
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
