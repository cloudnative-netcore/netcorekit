using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using MediatR;
using Microsoft.Extensions.Configuration;
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

    public EventBus(IMediator mediator, IConfiguration config)
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
    }

    public async Task Publish(IEvent @event)
    {
      var data = JsonConvert.SerializeObject(@event, Formatting.Indented);
      await _producer.ProduceAsync(_topic, @event.GetType().AssemblyQualifiedName, data);
    }

    public async Task Subscribe<T>() where T : IEvent
    {
      using (var consumer = new Consumer<string, string>(
        constructConfig(_brokerList, true),
        new StringDeserializer(Encoding.UTF8),
        new StringDeserializer(Encoding.UTF8)))
      {
        consumer.OnPartitionEOF += (_, end)
          => Console.WriteLine(
            $"Reached end of topic {end.Topic} partition {end.Partition}, next message will be at offset {end.Offset}");

        consumer.OnError += (_, error)
          => Console.WriteLine($"Error: {error}");

        consumer.OnConsumeError += (_, msg)
          => Console.WriteLine(
            $"Error consuming from topic/partition/offset {msg.Topic}/{msg.Partition}/{msg.Offset}: {msg.Error}");

        consumer.OnOffsetsCommitted += (_, commit) =>
        {
          Console.WriteLine($"[{string.Join(", ", commit.Offsets)}]");

          if (commit.Error) Console.WriteLine($"Failed to commit offsets: {commit.Error}");
          Console.WriteLine($"Successfully committed offsets: [{string.Join(", ", commit.Offsets)}]");
        };

        consumer.OnPartitionsAssigned += (_, partitions) =>
        {
          Console.WriteLine($"Assigned partitions: [{string.Join(", ", partitions)}], member id: {consumer.MemberId}");
          consumer.Assign(partitions);
        };

        consumer.OnPartitionsRevoked += (_, partitions) =>
        {
          Console.WriteLine($"Revoked partitions: [{string.Join(", ", partitions)}]");
          consumer.Unassign();
        };

        consumer.OnStatistics += (_, json)
          => Console.WriteLine($"Statistics: {json}");

        consumer.Subscribe(_topic);

        Console.WriteLine($"Subscribed to: [{string.Join(", ", consumer.Subscription)}]");

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
          Console.WriteLine($"Topic: {msg.Topic} Partition: {msg.Partition} Offset: {msg.Offset} {msg.Value}");

          var eventType = Type.GetType(msg.Key);
          var domainEvent = (IEvent)JsonConvert.DeserializeObject(msg.Value, eventType);
          await _mediator.Publish(Mapper.Map<INotification>(domainEvent));
        }
      }
    }

    private static IDictionary<string, object> constructConfig(string brokerList, bool enableAutoCommit)
    {
      return new Dictionary<string, object>
      {
        ["group.id"] = "jambo-consumer",
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
