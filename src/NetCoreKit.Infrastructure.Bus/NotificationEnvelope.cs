using MediatR;
using NetCoreKit.Domain;

namespace NetCoreKit.Infrastructure.Bus
{
    public class NotificationEnvelope : INotification
    {
        public NotificationEnvelope(IEvent @event)
        {
            Event = @event;
        }

        public IEvent Event { get; }
    }
}
