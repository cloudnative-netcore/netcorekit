using System.Threading;
using MediatR;
using NetCoreKit.Samples.TodoAPI.Domain;

namespace NetCoreKit.Samples.TodoAPI.v1.Services
{
  public class EventSubscriber : INotificationHandler<ProjectCreated>
  {
    public async System.Threading.Tasks.Task Handle(ProjectCreated @event, CancellationToken cancellationToken)
    {
      // do something with @event
      //...

      await System.Threading.Tasks.Task.FromResult(@event);
    }
  }
}
