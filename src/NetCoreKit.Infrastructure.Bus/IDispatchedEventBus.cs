using System.Threading.Tasks;
using Google.Protobuf;

namespace NetCoreKit.Infrastructure.Bus
{
    public interface IDispatchedEventBus
    {
        Task PublishAsync<TMessage>(TMessage msg, params string[] chans) where TMessage : IMessage<TMessage>;
        Task SubscribeAsync<TMessage>(params string[] chans) where TMessage : IMessage<TMessage>, new();
    }
}
