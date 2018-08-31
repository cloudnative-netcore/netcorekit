using System;
using System.Collections.Generic;
using System.Linq;
using NetCoreKit.Domain;

namespace NetCoreKit.Samples.TodoAPI.Domain
{
  public class Project : AggregateRootBase
  {
    private Project()
    {
    }

    private Project(string name)
    {
      Name = name;
    }

    public string Name { get; private set; }
    public ICollection<Task> Tasks { get; set; } = new List<Task>();

    public static Project Load(string name)
    {
      return new Project(name);
    }

    public Project ChangeName(string name)
    {
      Name = name;
      return this;
    }

    public Project AddTask(Task task)
    {
      task.SetProject(this);
      Tasks.Add(task);
      return this;
    }

    public Project UpdateTask(Guid taskId, string taskName, int order = 1, bool completed = false)
    {
      var task = Tasks.FirstOrDefault(x => x.Id == taskId);
      if(task == null)
        throw new DomainException($"Couldn't find any task#{taskId}");

      task.ChangeTitle(taskName)
        .ChangeOrder(order);

      if (completed) task.ChangeToCompleted();

      return this;
    }

    public Project RemoveTask(Guid taskId)
    {
      Tasks = Tasks.Where(x => x.Id != taskId).ToList();
      return this;
    }

    public Project ClearTasks()
    {
      Tasks = new List<Task>();
      return this;
    }
  }
}
