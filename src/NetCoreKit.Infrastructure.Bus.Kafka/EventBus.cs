using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetCoreKit.Domain;
using Newtonsoft.Json;

namespace NetCoreKit.Infrastructure.Bus.Kafka
{
  /// <summary>
  ///   Source: https://github.com/ivanpaulovich/event-sourcing-jambo
  /// </summary>
  public class EventBus : IEventBus
  {
    private readonly string _brokerList;

    private readonly IMediator _mediator;

    private readonly Producer<string, string> _producer;
    private readonly string _topic;

    private readonly ILogger<EventBus> _logger;

    public EventBus(IMediator mediator, IConfiguration config, ILoggerFactory factory)
    {
      _brokerList = config.GetValue("EventBus:Brokers", "127.0.0.1:9092");
      _topic = config.GetValue("EventBus:Topic", "IAmKafka");

      _producer = new Producer<string, string>(
        new Dictionary<string, object>
        {
          {
            "bootstrap.servers",
            _brokerList
          }
        },
        new StringSerializer(Encoding.UTF8), new StringSerializer(Encoding.UTF8));

      _mediator = mediator;
      _logger = factory.CreateLogger<EventBus>();
    }

    public async Task Publish(IEvent @event)
    {
      var data = JsonConvert.SerializeObject(@event, Formatting.Indented);
      await _producer.ProduceAsync(_topic, @event.GetType().AssemblyQualifiedName, data);
    }

    public async Task Subscribe<TEvent>() where TEvent : IEvent
    {
      using (var consumer = new Consumer<string, string>(
        constructConfig(_brokerList, true),
        new StringDeserializer(Encoding.UTF8),
        new StringDeserializer(Encoding.UTF8)))
      {
        consumer.OnPartitionEOF += (_, end)
          => _logger.LogInformation(
            $"Reached end of topic {end.Topic} partition {end.Partition}, next message will be at offset {end.Offset}");

        consumer.OnError += (_, error)
          => _logger.LogError($"Error: {error}");

        consumer.OnConsumeError += (_, msg)
          => _logger.LogError(
            $"Error consuming from topic/partition/offset {msg.Topic}/{msg.Partition}/{msg.Offset}: {msg.Error}");

        consumer.OnOffsetsCommitted += (_, commit) =>
        {
          _logger.LogInformation($"[{string.Join(", ", commit.Offsets)}]");

          if (commit.Error)
            _logger.LogError($"Failed to commit offsets: {commit.Error}");
          _logger.LogInformation($"Successfully committed offsets: [{string.Join(", ", commit.Offsets)}]");
        };

        consumer.OnPartitionsAssigned += (_, partitions) =>
        {
          _logger.LogInformation($"Assigned partitions: [{string.Join(", ", partitions)}], member id: {consumer.MemberId}");
          consumer.Assign(partitions);
        };

        consumer.OnPartitionsRevoked += (_, partitions) =>
        {
          _logger.LogInformation($"Revoked partitions: [{string.Join(", ", partitions)}]");
          consumer.Unassign();
        };

        consumer.OnStatistics += (_, json)
          => _logger.LogInformation($"Statistics: {json}");

        consumer.Subscribe(_topic);

        _logger.LogInformation($"Subscribed to: [{string.Join(", ", consumer.Subscription)}]");

        var cancelled = false;
        Console.CancelKeyPress += (_, e) =>
        {
          e.Cancel = true; // prevent the process from terminating.
          cancelled = true;
        };

        Console.WriteLine("Ctrl-C to exit.");
        while (!cancelled)
        {
          if (!consumer.Consume(out var msg, TimeSpan.FromSeconds(1))) continue;
          _logger.LogInformation($"Topic: {msg.Topic} Partition: {msg.Partition} Offset: {msg.Offset} {msg.Value}");

          var domainEvent = (TEvent)JsonConvert.DeserializeObject(msg.Value, typeof(TEvent));
          await _mediator.Publish(Mapper.Map<INotification>(domainEvent));
        }
      }
    }

    private static IDictionary<string, object> constructConfig(string brokerList, bool enableAutoCommit)
    {
      return new Dictionary<string, object>
      {
        ["group.id"] = "netcorekit-consumer",
        ["enable.auto.commit"] = enableAutoCommit,
        ["auto.commit.interval.ms"] = 5000,
        ["statistics.interval.ms"] = 60000,
        ["bootstrap.servers"] = brokerList,
        ["default.topic.config"] = new Dictionary<string, object>
        {
          ["auto.offset.reset"] = "smallest"
        }
      };
    }
  }
}
