using NetCoreKit.Domain;

namespace NetCoreKit.Samples.Contracts.Events
{
  public class TaskCreated : EventBase
  {
    public TaskCreated(string title)
    {
      Title = title;
    }
    public string Title { get; }
  }
}
