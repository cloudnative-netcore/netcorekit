using System;
using System.Threading.Tasks;

namespace NetCoreKit.Domain
{
  public interface IDomainEventBus : IDisposable
  {
    Task Publish(IEvent @event);
  }

  public class MemoryDomainEventBus : IDomainEventBus
  {
    public void Dispose()
    {
    }

    public Task Publish(IEvent @event)
    {
      return Task.CompletedTask;
    }
  }
}
