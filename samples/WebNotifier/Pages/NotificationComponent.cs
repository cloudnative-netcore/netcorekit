using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazor.Extensions;
using Microsoft.AspNetCore.Blazor.Components;

namespace WebNotifier.Pages
{
  public class NotificationComponent : BlazorComponent
  {
    [Inject] public Configuration Config { get; set; }

    private HubConnection _connection;

    public List<ProjectModel> Projects { get; set; } = new List<ProjectModel>();
    public List<string> Messages { get; set; } = new List<string>();

    protected override async Task OnInitAsync()
    {
      var url = Config.SignalRBaseUrl ?? "http://localhost:5002";

      _connection = new HubConnectionBuilder()
        .WithUrl($"{url}/project",
          opt =>
          {
            opt.LogLevel = SignalRLogLevel.Trace;
            opt.Transport = HttpTransportType.WebSockets;
          })
        .Build();

      _connection.On<ProjectCreatedDto>("projectCreatedNotify", HandleProjectCreated);
      _connection.On<TaskCreatedDto>("taskAddedToProjectNotify", HandleTaskCreated);

      _connection.OnClose(exc => Task.CompletedTask);
      await _connection.StartAsync();
    }

    private Task HandleProjectCreated(ProjectCreatedDto msg)
    {
      Messages.Add($"{msg.Name} project created at {msg.OccurredOn}.");

      if (Projects.Any(p => p.Id == msg.Id)) return Task.CompletedTask;

      Projects.Add(new ProjectModel
      {
        Id = msg.Id,
        Name = msg.Name
      });

      StateHasChanged();

      return Task.CompletedTask;
    }

    private Task HandleTaskCreated(TaskCreatedDto msg)
    {
      Messages.Add($"Task {msg.Title} created at {msg.OccurredOn}.");

      var tasks = Projects.SelectMany(p => p.Tasks);
      if (tasks.Any(t => t.Id == msg.Id))
        return Task.CompletedTask;

      var project = Projects.FirstOrDefault(p => p.Id == msg.ProjectId);
      if(project == null)
        return Task.CompletedTask;

      project.Tasks.Add(new ProjectModel.TaskModel
      {
        Id = msg.Id,
        Title = msg.Title
      });

      StateHasChanged();
      return Task.CompletedTask;
    }
  }

  public class ProjectModel
  {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<TaskModel> Tasks { get; set; } = new List<TaskModel>();
    public class TaskModel
    {
      public Guid Id { get; set; }
      public string Title { get; set; }
    }
  }

  public class ProjectCreatedDto
  {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime OccurredOn { get; set; }
  }

  public class TaskCreatedDto
  {
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Title { get; set; }
    public DateTime OccurredOn { get; set; }
  }
}
