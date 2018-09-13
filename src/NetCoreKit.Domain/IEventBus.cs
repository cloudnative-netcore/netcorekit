using System;
using System.Threading.Tasks;

namespace NetCoreKit.Domain
{
  public interface IEventBus : IDisposable
  {
    Task Publish(IEvent @event, params string[] topics);
    Task Subscribe(params string[] topics);
    Task SubscribeAsync(params string[] topics);
  }
}
