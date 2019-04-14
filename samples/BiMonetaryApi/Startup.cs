using Grpc.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCoreKit.RestTemplate.MongoDb;
using static NetCoreKit.Samples.BiMonetaryApi.Rpc.ExchangeService;

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
                var client = new ExchangeServiceClient(channel);
                services.AddSingleton(typeof(ExchangeServiceClient), client);
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMongoDbTemplate();
        }
    }
}
