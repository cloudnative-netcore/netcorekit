using System;
using NetCoreKit.Domain;

namespace NetCoreKit.Samples.TodoAPI.Domain
{
    public class TaskCreated : EventBase
    {
        public TaskCreated(Guid id, string title, Guid projectId)
        {
            Id = id;
            Title = title;
            ProjectId = projectId;
        }

        public Guid Id { get; }
        public string Title { get; }
        public Guid ProjectId { get; }
    }
}
