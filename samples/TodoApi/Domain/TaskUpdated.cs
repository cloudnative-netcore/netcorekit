using MediatR;
using NetCoreKit.Domain;

namespace NetCoreKit.Samples.TodoAPI.Domain
{
    public class TaskUpdated : EventBase, INotification
    {
    }
}
