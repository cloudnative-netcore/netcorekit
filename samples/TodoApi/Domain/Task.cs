using System;
using System.ComponentModel.DataAnnotations;
using NetCoreKit.Domain;
using static NetCoreKit.Utils.Helpers.IdHelper;

namespace NetCoreKit.Samples.TodoAPI.Domain
{
    public sealed class Task : EntityBase
    {
        private Task()
        {
        }

        private Task(string title)
            : this(GenerateId(), title)
        {
        }

        private Task(Guid id, string title) : base(id)
        {
            Title = title;
        }

        public int? Order { get; private set; } = 1;
        [Required] public string Title { get; private set; }
        public bool? Completed { get; private set; } = false;
        public Guid AuthorId { get; private set; }
        public string AuthorName { get; private set; }
        public Project Project { get; private set; }
        public Guid ProjectId { get; private set; }

        public static Task Load(string title)
        {
            return new Task(title);
        }

        public static Task Load(Guid id, string title)
        {
            return new Task(id, title);
        }

        public Task ChangeTitle(string title)
        {
            if (string.IsNullOrEmpty(title))
                throw new DomainException("Title is null or empty.");

            Title = title;
            return this;
        }

        public Task ChangeOrder(int order)
        {
            if (order <= 0)
                throw new DomainException("Order could be greater than zero.");

            Order = order;
            return this;
        }

        public Task ChangeToCompleted()
        {
            Completed = true;
            return this;
        }

        public Task SetAuthor(Guid id, string authorName)
        {
            AuthorId = id;
            AuthorName = authorName;
            return this;
        }

        public Task SetProject(Project project)
        {
            Project = project;
            ProjectId = project.Id;
            return this;
        }
    }
}
