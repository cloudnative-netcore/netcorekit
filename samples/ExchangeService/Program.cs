using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetCoreKit.Infrastructure.Host.gRPC;
using NetCoreKit.Samples.ExchangeService.Rpc;
using NetCoreKit.Template.gRPC.Standard;

namespace NetCoreKit.Samples.ExchangeService
{
    public class Program
    {
        public static IConfiguration Configuration { get; set; }

        public static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureDefaultSettings(args, svc => { svc.AddHostedService<HostedService>(); });

            await host.RunAsync();
        }
    }

    public class HostedService : HostedServiceBase
    {
        public HostedService(ILoggerFactory loggerFactory, IApplicationLifetime appLifetime, IConfiguration config)
            : base(loggerFactory, appLifetime, config)
        {
        }

        protected override Server ConfigureServer()
        {
            var host = Config["Hosts:Local:Host"];
            var port = int.Parse(Config["Hosts:Local:Port"]);

            var server = new Server
            {
                Services =
                {
                    BiMonetaryApi.Rpc.ExchangeService.BindService(new ExchangeServiceImpl(LoggerFactory)),
                    Grpc.Health.V1.Health.BindService(new HealthImpl())
                },
                Ports = {new ServerPort(host, port, ServerCredentials.Insecure)}
            };

            Logger.LogInformation($"{nameof(BiMonetaryApi.Rpc.ExchangeService)} is listening on {host}:{port}.");
            return server;
        }

        protected override void SuppressFinalize()
        {
        }
    }
}
