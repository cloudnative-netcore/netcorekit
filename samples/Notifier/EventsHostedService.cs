using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetCoreKit.Domain;
using NetCoreKit.Infrastructure.Mappers;

namespace NetCoreKit.Samples.Notifier
{
  internal class EventsHostedService : IHostedService
  {
    private readonly ILogger _logger;
    private readonly IApplicationLifetime _appLifetime;
    private readonly IEventBus _eventBus;

    public EventsHostedService(
      ILogger<EventsHostedService> logger,
      IApplicationLifetime appLifetime,
      IEventBus eventBus)
    {
      _logger = logger;
      _appLifetime = appLifetime;
      _eventBus = eventBus;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
      _appLifetime.ApplicationStarted.Register(OnStarted);
      _appLifetime.ApplicationStopping.Register(OnStopping);
      _appLifetime.ApplicationStopped.Register(OnStopped);

      return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      return Task.CompletedTask;
    }

    private void OnStarted()
    {
      _logger.LogInformation("OnStarted has been called.");

      _eventBus.Subscribe<ProjectCreated>().Wait();
    }

    private void OnStopping()
    {
      _logger.LogInformation("OnStopping has been called.");
    }

    private void OnStopped()
    {
      _logger.LogInformation("OnStopped has been called.");

      if(_eventBus != null)
        GC.SuppressFinalize(_eventBus);
    }
  }

  public class ProjectCreatedProfile : Profile
  {
    public ProjectCreatedProfile()
    {
      this.MapMySelf(typeof(ProjectCreated));
    }
  }

  public class ProjectCreated : EventBase, INotification
  {
    public ProjectCreated(string name)
    {
      Name = name;
    }

    public string Name { get; set; }
  }

  public class ProjectCreatedSubscriber : INotificationHandler<ProjectCreated>
  {
    private readonly ILogger _logger;

    public ProjectCreatedSubscriber(ILogger<ProjectCreatedSubscriber> logger)
    {
      _logger = logger;
    }

    public Task Handle(ProjectCreated @event, CancellationToken cancellationToken)
    {
      _logger.LogInformation($"@ Project Created Event -{@event.OccurredOn}-{@event.EventVersion}. Now I will do something cool in this worker...");

      return Task.FromResult(@event);
    }
  }
}
