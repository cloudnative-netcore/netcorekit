using System;
using System.Collections.Generic;
using System.Linq;
using NetCoreKit.Domain;

namespace NetCoreKit.Samples.TodoAPI.Domain
{
  public class Project : EntityBase
  {
    internal Project()
    {
    }

    private Project(string name)
    {
      Name = name;
    }

    public string Name { get; private set; }
    public ICollection<Task> Tasks { get; private set; } = new List<Task>();

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
      Tasks.Add(task.SetProject(this));
      return this;
    }

    public Project UpdateTask(Task task)
    {
      var taskInList = Tasks.First(x => x.Id == task.Id);
      if (!Tasks.Remove(task)) throw new DomainException("Couldn't remove the old task.");
      Tasks.Add(taskInList);
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
