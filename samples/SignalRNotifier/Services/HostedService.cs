using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace NetCoreKit.Samples.SignalRNotifier.Services
{
  /// <summary>
  ///   Source: https://github.com/elucidsoft/aspnetcore-Vue-starter-signalR/blob/master/Services/HostedService.cs
  /// </summary>
  public abstract class HostedService : IHostedService
  {
    private CancellationTokenSource _cts;
    private Task _executingTask;

    public Task StartAsync(CancellationToken cancellationToken)
    {
      _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

      _executingTask = ExecuteAsync(_cts.Token);

      return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
      if (_executingTask == null) return;

      _cts.Cancel();

      await Task.WhenAny(_executingTask, Task.Delay(-1, cancellationToken));

      cancellationToken.ThrowIfCancellationRequested();
    }

    protected abstract Task ExecuteAsync(CancellationToken cancellationToken);
  }
}
