using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace NetCoreKit.Infrastructure.Host.Grpc
{
    public abstract class HostedServiceBase : IHostedService
    {
        protected readonly ILogger<HostedServiceBase> Logger;
        protected readonly ILoggerFactory LoggerFactory;

        protected readonly IApplicationLifetime AppLifetime;
        protected readonly IConfiguration Config;

        protected HostedServiceBase(
            ILoggerFactory loggerFactory,
            IApplicationLifetime appLifetime,
            IConfiguration config)
        {
            Logger = loggerFactory.CreateLogger<HostedServiceBase>();
            LoggerFactory = loggerFactory;
            AppLifetime = appLifetime;
            Config = config;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            AppLifetime.ApplicationStarted.Register(OnStarted);
            AppLifetime.ApplicationStopping.Register(OnStopping);
            AppLifetime.ApplicationStopped.Register(OnStopped);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected virtual void OnStarted()
        {
            ConfigureServer().Start();
            Logger.LogInformation("OnStarted has been called.");
        }

        protected virtual void OnStopping()
        {
            Logger.LogInformation("OnStopping has been called.");
        }

        protected virtual void OnStopped()
        {
            SuppressFinalize();
            Logger.LogInformation("OnStopped has been called.");
        }

        protected abstract Server ConfigureServer();
        protected abstract void SuppressFinalize();
    }
}
