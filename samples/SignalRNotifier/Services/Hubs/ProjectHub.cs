using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
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

    public ProjectHostService(IHubContext<ProjectHub> context, IDispatchedEventBus eventBus)
    {
      _eventBus = eventBus;
      Clients = context.Clients;
    }

    private IHubClients Clients { get; }

    public async Task Handle(Notifications.ProjectCreated notification, CancellationToken cancellationToken)
    {
      await Clients.All.SendAsync("projectCreatedNotify", notification, cancellationToken);
    }

    public async Task Handle(Notifications.TaskCreated notification, CancellationToken cancellationToken)
    {
      await Clients.All.SendAsync("taskAddedToProjectNotify", notification, cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
      Task.Run(() => _eventBus.Subscribe<ProjectCreatedMsg>("project-created"), cancellationToken);
      Task.Run(() => _eventBus.Subscribe<TaskCreatedMsg>("task-created"), cancellationToken);
      return Task.CompletedTask;
    }
  }
}
