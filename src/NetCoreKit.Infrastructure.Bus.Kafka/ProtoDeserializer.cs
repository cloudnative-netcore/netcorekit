using System.Collections.Generic;
using Confluent.Kafka.Serialization;
using Google.Protobuf;

namespace NetCoreKit.Infrastructure.Bus.Kafka
{
  public class ProtoDeserializer<T> : IDeserializer<T>
    where T : IMessage<T>, new()
  {
    private readonly MessageParser<T> _parser;

    public ProtoDeserializer()
    {
      _parser = new MessageParser<T>(() => new T());
    }

    public IEnumerable<KeyValuePair<string, object>>
      Configure(IEnumerable<KeyValuePair<string, object>> config, bool isKey)
    {
      return config;
    }

    public void Dispose()
    {
    }

    public T Deserialize(string topic, byte[] data)
    {
      return _parser.ParseFrom(data);
    }
  }
}
