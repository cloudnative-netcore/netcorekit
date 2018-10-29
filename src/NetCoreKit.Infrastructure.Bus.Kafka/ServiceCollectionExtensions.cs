using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NetCoreKit.Infrastructure.Bus.Kafka
{
  public static class ServiceCollectionExtensions
  {
    public static IServiceCollection AddKafkaEventBus(this IServiceCollection services)
    {
      var resolver = services.BuildServiceProvider();
      using (var scope = resolver.CreateScope())
      {
        var config = scope.ServiceProvider.GetService<IConfiguration>();
        var env = scope.ServiceProvider.GetService<IHostingEnvironment>();
        var kafkaOptions = config.GetSection("Kafka");
        //if (env.IsDevelopment())
        {
          services.Configure<KafkaOptions>(o => { o.Fqdn = kafkaOptions.GetValue<string>("FQDN"); });
        }
        /*else
        {
          var serviceName = kafkaOptions
            .GetValue("ServiceName", "kafka")
            .Replace("-", "_")
            .ToUpperInvariant();

          var ip = Environment.GetEnvironmentVariable($"{serviceName}_SERVICE_HOST");
          var port = Environment.GetEnvironmentVariable($"{serviceName}_SERVICE_PORT");

          services.Configure<KafkaOptions>(o => { o.Fqdn = $"{ip}:{port}"; });
        }*/

        services.AddSingleton<IDispatchedEventBus, DispatchedEventBus>();
      }

      return services;
    }
  }
}
