using System.Threading;
using MediatR;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<KafkaEnvelopeEventHandler> _logger;

    public KafkaEnvelopeEventHandler(IDispatchedEventBus eventBus, ILoggerFactory loggerFactory)
    {
      _eventBus = eventBus;
      _logger = loggerFactory.CreateLogger<KafkaEnvelopeEventHandler>();
    }

    public async Task Handle(NotificationEnvelope notify, CancellationToken cancellationToken)
    {
      if (notify.Event is ProjectCreated created)
      {
        _logger.LogInformation("[NCK] Start to publish ProjectCreatedMsg.");
        await _eventBus.Publish(created.MapTo<ProjectCreated, ProjectCreatedMsg>(), "project-created");
      }
      else if(notify.Event is TaskCreated taskCreated)
      {
        _logger.LogInformation("[NCK] Start to publish TaskCreatedMsg.");
        await _eventBus.Publish(taskCreated.MapTo<TaskCreated, TaskCreatedMsg>(), "task-created");
      }
    }
  }
}
