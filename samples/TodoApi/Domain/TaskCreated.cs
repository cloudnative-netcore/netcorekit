using NetCoreKit.Domain;

namespace NetCoreKit.Samples.TodoAPI.Domain
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
