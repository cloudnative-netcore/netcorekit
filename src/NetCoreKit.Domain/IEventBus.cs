using System.Threading.Tasks;

namespace NetCoreKit.Domain
{
  public interface IEventBus
  {
    Task Publish(IEvent @event);
    Task Subscribe<T>() where T : IEvent;
  }
}
