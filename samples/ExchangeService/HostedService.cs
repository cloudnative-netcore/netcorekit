using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Reflection;
using Grpc.Reflection.V1Alpha;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace NetCoreKit.Samples.ExchangeService
{
    public class HostedService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IApplicationLifetime _appLifetime;
        private readonly IConfiguration _config;

        public HostedService(
            ILogger<HostedService> logger,
            IApplicationLifetime appLifetime,
            IConfiguration config)
        {
            _logger = logger;
            _appLifetime = appLifetime;
            _config = config;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _appLifetime.ApplicationStarted.Register(OnStarted);
            _appLifetime.ApplicationStopping.Register(OnStopping);
            _appLifetime.ApplicationStopped.Register(OnStopped);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void OnStarted()
        {
            _logger.LogInformation("OnStarted has been called.");

            var port = int.Parse(_config["service:port"]);

            var refImpl = new ReflectionServiceImpl(
                ServerReflection.Descriptor, Rpc.ExchangeService.Descriptor);

            var server = new Server
            {
                Services =
                {
                    Rpc.ExchangeService.BindService(new Rpc.ExchangeServiceImpl()),
                    ServerReflection.BindService(refImpl)
                },
                Ports = {new ServerPort("localhost", port, ServerCredentials.Insecure)}
            };

            server.Start();

            _logger.LogInformation($"Exchange Service gRPC service listening on port {port}.");
        }

        private void OnStopping()
        {
            _logger.LogInformation("OnStopping has been called.");
        }

        private void OnStopped()
        {
            _logger.LogInformation("OnStopped has been called.");

        }
    }
}
