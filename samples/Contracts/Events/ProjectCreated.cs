using NetCoreKit.Domain;

namespace NetCoreKit.Samples.Contracts.Events
{
  public class ProjectCreated : EventBase
  {
    public ProjectCreated(string name)
    {
      Name = name;
    }

    public string Name { get; }
  }
}
