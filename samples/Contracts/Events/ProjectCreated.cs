using MediatR;
using NetCoreKit.Domain;

namespace NetCoreKit.Samples.Contracts.Events
{
  public class ProjectCreated : EventBase, INotification
  {
    public ProjectCreated(string name)
    {
      Name = name;
    }

    public string Name { get; }
  }
}
