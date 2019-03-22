using System;
using MediatR;

namespace NetCoreKit.Samples.SignalRNotifier.Services.Events
{
    public class ProjectCreated : INotification
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime OccurredOn { get; set; }
    }
}
