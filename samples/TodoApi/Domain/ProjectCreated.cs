using System;
using NetCoreKit.Domain;

namespace NetCoreKit.Samples.TodoAPI.Domain
{
    public class ProjectCreated : EventBase
    {
        public ProjectCreated(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; }
        public string Name { get; }
    }
}
