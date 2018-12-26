using System;
using System.Threading;
using System.Threading.Tasks;
using static NetCoreKit.Utils.Helpers.DateTimeHelper;

namespace NetCoreKit.Domain
{
    /// <summary>
    ///     Supertype for all Event types
    /// </summary>
    public interface IEvent
    {
        int EventVersion { get; }
        DateTime OccurredOn { get; }
    }

    public interface IEventHandler<in TEvent, TResult>
        where TEvent : IEvent
    {
        Task<TResult> Handle(TEvent request, CancellationToken cancellationToken);
    }

    public interface IDomainEventBus : IDisposable
    {
        Task Publish(IEvent @event);
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

        public IEvent Event { get; }
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
