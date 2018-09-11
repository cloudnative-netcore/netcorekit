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
        .WithUrl("https://localhost:44398/project",
          opt =>
          {
            opt.LogLevel = SignalRLogLevel.Trace;
            opt.Transport = HttpTransportType.WebSockets;
          })
        .Build();

      _connection.On<object>("projectCreatedNotify", HandleProjectCreated);
      _connection.On<object>("taskAddedToProjectNotify", HandleTaskCreated);

      _connection.OnClose(exc => Task.CompletedTask);
      await _connection.StartAsync();
    }

    private Task HandleProjectCreated(object msg)
    {
      Messages.Add(msg.ToString());
      StateHasChanged();
      return Task.CompletedTask;
    }

    private Task HandleTaskCreated(object msg)
    {
      Messages.Add(msg.ToString());
      StateHasChanged();
      return Task.CompletedTask;
    }
  }
}
