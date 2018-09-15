using System.Threading.Tasks;
using Google.Protobuf;

namespace NetCoreKit.Infrastructure.Bus.Kafka
{
  public interface IDispatchedEventBus
  {
    Task Publish<TMessage>(TMessage msg, params string[] topics) where TMessage : IMessage<TMessage>;
    Task Subscribe<TMessage>(params string[] topics) where TMessage : IMessage<TMessage>, new();
  }
}
