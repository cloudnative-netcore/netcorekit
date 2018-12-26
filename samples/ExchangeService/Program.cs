using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Grpc.Core;
using Grpc.Reflection;
using Grpc.Reflection.V1Alpha;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetCoreKit.Infrastructure.Host.gRPC;
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
            var port = int.Parse(Config["Hosts:Local:Port"]);
            var refImpl = new ReflectionServiceImpl(ServerReflection.Descriptor,
                BiMonetaryApi.Rpc.ExchangeService.Descriptor);

            var server = new Server
            {
                Services =
                {
                    BiMonetaryApi.Rpc.ExchangeService.BindService(new Rpc.ExchangeServiceImpl(LoggerFactory)),
                    ServerReflection.BindService(refImpl)
                },
                Ports = {new ServerPort("localhost", port, ServerCredentials.Insecure)}
            };

            Logger.LogInformation($"{nameof(BiMonetaryApi.Rpc.ExchangeService)} is listening on port {port}.");
            return server;
        }

        protected override void SuppressFinalize()
        {
        }
    }
}
