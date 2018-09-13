using System;
using MediatR;

namespace NetCoreKit.Samples.SignalRNotifier.Services.Notifications
{
  public class TaskCreated : INotification
  {
    public string Title { get; set; }
    public DateTime OccurredOn { get; set; }
  }
}
