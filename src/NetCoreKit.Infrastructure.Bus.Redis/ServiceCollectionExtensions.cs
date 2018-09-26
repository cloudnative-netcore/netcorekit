using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NetCoreKit.Infrastructure.Bus.Redis
{
  public static class ServiceCollectionExtensions
  {
    public static IServiceCollection AddRedisBus(this IServiceCollection services)
    {
      var resolver = services.BuildServiceProvider();
      using (var scope = resolver.CreateScope())
      {
        var config = scope.ServiceProvider.GetService<IConfiguration>();
        var redisOptions = config.GetSection("Redis");

        services.Configure<RedisOptions>(o =>
        {
          o.Connection = redisOptions.GetValue<string>("Connection");
          o.Password = redisOptions.GetValue<string>("Password");
        });

        services.AddSingleton<RedisStore>();
        services.AddSingleton<IDispatchedEventBus, DispatchedEventBus>();
        return services;
      }
    }
  }
}
