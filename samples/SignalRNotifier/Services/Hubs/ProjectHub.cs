using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using NetCoreKit.Domain;
using NetCoreKit.Samples.Contracts.Events;

namespace NetCoreKit.Samples.SignalRNotifier.Services.Hubs
{
  public class ProjectHub : Hub
  {
  }

  public class ProjectHostService : HostedService,
    INotificationHandler<ProjectCreated>,
    INotificationHandler<TaskCreated>
  {
    private readonly IEventBus _eventBus;
    private readonly IServiceProvider _resolver;

    public ProjectHostService(IHubContext<ProjectHub> context, IServiceProvider resolver, IEventBus eventBus)
    {
      _resolver = resolver;
      _eventBus = eventBus;
      Clients = context.Clients;
    }

    private IHubClients Clients { get; }

    public async Task Handle(ProjectCreated @event, CancellationToken cancellationToken)
    {
      await Clients.All.SendAsync("projectCreatedNotify", @event, cancellationToken);
    }

    public async Task Handle(TaskCreated @event, CancellationToken cancellationToken)
    {
      await Clients.All.SendAsync("taskAddedToProjectNotify", @event, cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
      _eventBus.Subscribe("project");
      return Task.CompletedTask;
    }
  }
}
