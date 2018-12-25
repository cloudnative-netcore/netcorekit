using System.Threading;
using MediatR;
using Microsoft.Extensions.Logging;
using NetCoreKit.Infrastructure.Bus;
using NetCoreKit.Infrastructure.Mappers;
using NetCoreKit.Samples.Contracts.TodoApi;
using NetCoreKit.Samples.TodoAPI.Domain;
using Task = System.Threading.Tasks.Task;

namespace NetCoreKit.Samples.TodoApi.v1.PubSub
{
    public class RedisNotifPublisher : INotificationHandler<NotificationEnvelope>
    {
        private readonly IDispatchedEventBus _dispatchedEventBus;
        private readonly ILogger<RedisNotifPublisher> _logger;

        public RedisNotifPublisher(IDispatchedEventBus dispatchedEventBus, ILoggerFactory loggerFactory)
        {
            _dispatchedEventBus = dispatchedEventBus;
            _logger = loggerFactory.CreateLogger<RedisNotifPublisher>();
        }

        public async Task Handle(NotificationEnvelope notify, CancellationToken cancellationToken)
        {
            switch (notify.Event)
            {
                case ProjectCreated projectCreated:
                    _logger.LogInformation("[NCK] Start to publish ProjectCreatedMsg.");
                    await _dispatchedEventBus.PublishAsync(
                        projectCreated.MapTo<ProjectCreated, ProjectCreatedMsg>(),
                        "project-created");
                    break;
                case TaskCreated taskCreated:
                    _logger.LogInformation("[NCK] Start to publish TaskCreatedMsg.");
                    await _dispatchedEventBus.PublishAsync(
                        taskCreated.MapTo<TaskCreated, TaskCreatedMsg>(),
                        "task-created");
                    break;
            }
        }
    }
}
