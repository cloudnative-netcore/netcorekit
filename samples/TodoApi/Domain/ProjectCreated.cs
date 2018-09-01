using MediatR;
using NetCoreKit.Domain;

namespace NetCoreKit.Samples.TodoAPI.Domain
{
  public class ProjectCreated : EventBase, INotification
  {
  }
}
