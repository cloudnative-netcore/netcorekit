using System.Threading;
using System.Threading.Tasks;

namespace NetCoreKit.Domain
{
  public interface IEventHandler<in TEvent, TResult>
    where TEvent : IEvent
  {
    Task<TResult> Handle(TEvent request, CancellationToken cancellationToken);
  }
}
