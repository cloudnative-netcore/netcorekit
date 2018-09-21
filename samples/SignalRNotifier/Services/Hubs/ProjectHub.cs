using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using NetCoreKit.Infrastructure.Bus.Kafka;
using Project.Proto;

namespace NetCoreKit.Samples.SignalRNotifier.Services.Hubs
{
  public class ProjectHub : Hub
  {
  }

  public class ProjectHostService : HostedService,
    INotificationHandler<Notifications.ProjectCreated>,
    INotificationHandler<Notifications.TaskCreated>
  {
    private readonly IDispatchedEventBus _eventBus;
    private readonly ILogger<ProjectHostService> _logger;

    public ProjectHostService(IHubContext<ProjectHub> context, IDispatchedEventBus eventBus, ILoggerFactory loggerFactory)
    {
      _eventBus = eventBus;
      Clients = context.Clients;
      _logger = loggerFactory.CreateLogger<ProjectHostService>();
    }

    private IHubClients Clients { get; }

    public async Task Handle(Notifications.ProjectCreated notification, CancellationToken cancellationToken)
    {
      _logger.LogInformation("Pushing message to projectCreatedNotify...");
      await Clients.All.SendAsync("projectCreatedNotify", notification, cancellationToken);
    }

    public async Task Handle(Notifications.TaskCreated notification, CancellationToken cancellationToken)
    {
      _logger.LogInformation("Pushing message to taskAddedToProjectNotify...");
      await Clients.All.SendAsync("taskAddedToProjectNotify", notification, cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
      Task.Run(() =>
      {
        _logger.LogInformation("[NCK] Start to subscribe to project-created...");
        return _eventBus.Subscribe<ProjectCreatedMsg>("project-created");
      }, cancellationToken);

      Task.Run(() =>
      {
        _logger.LogInformation("[NCK] Start to subscribe to task-created...");
        return _eventBus.Subscribe<TaskCreatedMsg>("task-created");
      }, cancellationToken);

      return Task.CompletedTask;
    }
  }
}
