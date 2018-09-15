using System.Threading;
using MediatR;
using NetCoreKit.Domain;
using NetCoreKit.Infrastructure.Bus;
using NetCoreKit.Infrastructure.Bus.Kafka;
using NetCoreKit.Infrastructure.Mappers;
using NetCoreKit.Samples.TodoAPI.Domain;
using Project.Proto;
using Task = System.Threading.Tasks.Task;

namespace NetCoreKit.Samples.TodoAPI.v1.Services
{
  public class KafkaEnvelopeEventHandler : INotificationHandler<NotificationEnvelope>
  {
    private readonly IDispatchedEventBus _eventBus;

    public KafkaEnvelopeEventHandler(IDispatchedEventBus eventBus)
    {
      _eventBus = eventBus;
    }

    public Task Handle(NotificationEnvelope notify, CancellationToken cancellationToken)
    {

      if (notify.Event is ProjectCreated)
      {
        var msg = notify.Event.MapTo<IEvent, ProjectCreatedMsg>();
        _eventBus.Publish(msg, "project");
      }

      return Task.CompletedTask;
    }
  }
}
