using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using NetCoreKit.Infrastructure.Bus;
using NetCoreKit.Samples.SignalRNotifier.Services.Events;
using Project.Proto;

namespace NetCoreKit.Samples.SignalRNotifier.Services.Hubs
{
    public class ProjectHub : Hub
    {
    }

    public class ProjectHostService : HostedService,
        INotificationHandler<ProjectCreated>,
        INotificationHandler<TaskCreated>
    {
        private readonly IDispatchedEventBus _dispatchedEventBus;
        private readonly ILogger<ProjectHostService> _logger;

        public ProjectHostService(
            IHubContext<ProjectHub> context,
            IDispatchedEventBus dispatchedEventBus,
            ILoggerFactory loggerFactory)
        {
            Clients = context.Clients;
            _dispatchedEventBus = dispatchedEventBus;
            _logger = loggerFactory.CreateLogger<ProjectHostService>();
        }

        private IHubClients Clients { get; }

        public async Task Handle(ProjectCreated notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Pushing message to projectCreatedNotify...");
            await Clients.All.SendAsync("projectCreatedNotify", notification, cancellationToken);
        }

        public async Task Handle(TaskCreated notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Pushing message to taskAddedToProjectNotify...");
            await Clients.All.SendAsync("taskAddedToProjectNotify", notification, cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            RunAll(
                () => _dispatchedEventBus.SubscribeAsync<ProjectCreatedMsg>("project-created"),
                () => _dispatchedEventBus.SubscribeAsync<TaskCreatedMsg>("task-created")
            );
            return Task.CompletedTask;
        }

        private static void RunAll(params Action[] actions)
        {
            Task.WaitAll(actions.Select(action => Task.Factory.StartNew(action, TaskCreationOptions.LongRunning))
                .ToArray());
        }
    }
}
