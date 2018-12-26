using System.Collections.Generic;
using Confluent.Kafka.Serialization;
using Google.Protobuf;

namespace NetCoreKit.Infrastructure.Bus.Kafka
{
    public class ProtoSerializer<T> : ISerializer<T> where T : IMessage<T>
    {
        public IEnumerable<KeyValuePair<string, object>>
            Configure(IEnumerable<KeyValuePair<string, object>> config, bool isKey)
        {
            return config;
        }

        public void Dispose()
        {
        }

        public byte[] Serialize(string topic, T data)
        {
            return data.ToByteArray();
        }
    }
}
