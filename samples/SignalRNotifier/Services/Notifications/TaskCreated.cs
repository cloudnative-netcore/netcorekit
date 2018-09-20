using System;
using MediatR;

namespace NetCoreKit.Samples.SignalRNotifier.Services.Notifications
{
  public class TaskCreated : INotification
  {
    public Guid Id { get; set; }
    public string Title { get; set; }
    public Guid ProjectId { get; set; }
    public DateTime OccurredOn { get; set; }
  }
}
