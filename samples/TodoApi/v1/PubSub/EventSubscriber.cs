using System.Threading;
using MediatR;
using NetCoreKit.Samples.TodoAPI.Domain;
using Task = System.Threading.Tasks.Task;

namespace NetCoreKit.Samples.TodoApi.v1.PubSub
{
    public class EventSubscriber : INotificationHandler<ProjectCreated>
    {
        public async Task Handle(ProjectCreated @event, CancellationToken cancellationToken)
        {
            // do something with @event
            //...

            await Task.FromResult(@event);
        }
    }
}
