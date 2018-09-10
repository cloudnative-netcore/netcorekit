using MediatR;
using NetCoreKit.Domain;

namespace NetCoreKit.Samples.Contracts.Events
{
  public class TaskCreated : EventBase, INotification
  {
    public TaskCreated(string title)
    {
      Title = title;
    }
    public string Title { get; }
  }
}
