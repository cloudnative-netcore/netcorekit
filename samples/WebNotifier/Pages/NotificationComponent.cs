using System.Collections.Generic;
using System.Threading.Tasks;
using Blazor.Extensions;
using Microsoft.AspNetCore.Blazor.Components;

namespace WebNotifier.Pages
{
  public class NotificationComponent : BlazorComponent
  {
    private HubConnection _connection;
    public List<string> Messages { get; set; } = new List<string>();

    protected override async Task OnInitAsync()
    {
      _connection = new HubConnectionBuilder()
        .WithUrl($"{ApiClient.SignalRBaseUrl}/project",
          opt =>
          {
            opt.LogLevel = SignalRLogLevel.Trace;
            opt.Transport = HttpTransportType.WebSockets;
          })
        .Build();

      _connection.On<ProjectCreated>("projectCreatedNotify", HandleProjectCreated);
      _connection.On<TaskCreated>("taskAddedToProjectNotify", HandleTaskCreated);

      _connection.OnClose(exc => Task.CompletedTask);
      await _connection.StartAsync();
    }

    private Task HandleProjectCreated(ProjectCreated msg)
    {
      Messages.Add($"Project: {msg.Name}");
      StateHasChanged();
      return Task.CompletedTask;
    }

    private Task HandleTaskCreated(TaskCreated msg)
    {
      Messages.Add($"Task: {msg.Title}");
      StateHasChanged();
      return Task.CompletedTask;
    }
  }

  public class ProjectCreated
  {
    public string Name { get; set; }
  }

  public class TaskCreated
  {
    public string Title { get; set; }
  }
}
