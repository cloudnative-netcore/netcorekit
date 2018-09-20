using System.Threading;
using MediatR;
using NetCoreKit.Infrastructure.Bus;
using NetCoreKit.Infrastructure.Bus.Kafka;
using NetCoreKit.Infrastructure.Mappers;
using NetCoreKit.Samples.TodoAPI.Domain;
using Project.Proto;
using Task = System.Threading.Tasks.Task;

namespace NetCoreKit.Samples.TodoAPI.v1.UseCases.CreateProject
{
  public class KafkaEnvelopeEventHandler : INotificationHandler<NotificationEnvelope>
  {
    private readonly IDispatchedEventBus _eventBus;

    public KafkaEnvelopeEventHandler(IDispatchedEventBus eventBus)
    {
      _eventBus = eventBus;
    }

    public async Task Handle(NotificationEnvelope notify, CancellationToken cancellationToken)
    {
      if (notify.Event is ProjectCreated created)
      {
        await _eventBus.Publish(
          created.MapTo<ProjectCreated, ProjectCreatedMsg>(),
          "project-created");
      }
      else if(notify.Event is TaskCreated taskCreated)
      {
        await _eventBus.Publish(
          taskCreated.MapTo<TaskCreated, TaskCreatedMsg>(),
          "task-created");
      }
    }
  }
}
