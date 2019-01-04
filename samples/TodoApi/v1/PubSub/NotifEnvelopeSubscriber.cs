using System.Threading;
using MediatR;
using NetCoreKit.Infrastructure.Bus;
using Task = System.Threading.Tasks.Task;

namespace NetCoreKit.Samples.TodoApi.v1.PubSub
{
    public class NotifEnvelopeSubscriber : INotificationHandler<NotificationEnvelope>
    {
        public async Task Handle(NotificationEnvelope notif, CancellationToken cancellationToken)
        {
            // do something with @event
            //...

            await Task.FromResult(notif);
        }
    }
}
