using System;
using System.Threading.Tasks;

namespace NetCoreKit.Domain
{
  public interface IDomainEventBus : IDisposable
  {
    Task Publish(IEvent @event);
  }
}
