using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using NetCoreKit.Domain;

namespace NetCoreKit.Samples.SignalRNotifier.Services.Hubs
{
  public class ProjectHub : Hub
  {
  }

  public class ProjectHostService : HostedService,
    INotificationHandler<Notifications.ProjectCreated>,
    INotificationHandler<Notifications.TaskCreated>
  {
    private readonly IEventBus _eventBus;

    public ProjectHostService(IHubContext<ProjectHub> context, IEventBus eventBus)
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
      _eventBus.Subscribe("project").Wait(cancellationToken);
      return Task.CompletedTask;
    }
  }
}
