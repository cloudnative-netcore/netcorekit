using System;
using static NetCoreKit.Utils.Helpers.DateTimeHelper;

namespace NetCoreKit.Domain
{
  public interface IEvent
  {
    int EventVersion { get; }
    DateTime OccurredOn { get; }
  }

  public abstract class EventBase : IEvent
  {
    public int EventVersion { get; protected set; } = 1;
    public DateTime OccurredOn { get; protected set; } = GenerateDateTime();
  }

  public class EventEnvelope : EventBase
  {
    public EventEnvelope(IEvent @event)
    {
      Event = @event;
    }

    public IEvent Event { get;  }
  }
}
