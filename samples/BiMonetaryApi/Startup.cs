using Grpc.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCoreKit.Template.Rest.MongoDb;
using MyExchangeService = NetCoreKit.Samples.BiMonetaryApi.Rpc.ExchangeService;

namespace NetCoreKit.Samples.BiMonetaryApi
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMongoDbTemplate(null, (svc, resolver) =>
            {
                var config = resolver.GetService<IConfiguration>();
                var channel = new Channel(config["RpcClients:ExchangeService"], ChannelCredentials.Insecure);
                var client = new MyExchangeService.ExchangeServiceClient(channel);
                services.AddSingleton(typeof(MyExchangeService.ExchangeServiceClient), client);
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMongoDbTemplate();
        }
    }
}
