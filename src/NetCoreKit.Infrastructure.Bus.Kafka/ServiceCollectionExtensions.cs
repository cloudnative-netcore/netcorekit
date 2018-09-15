using Microsoft.Extensions.DependencyInjection;

namespace NetCoreKit.Infrastructure.Bus.Kafka
{
  public static class ServiceCollectionExtensions
  {
    public static IServiceCollection AddKafkaEventBus(this IServiceCollection services)
    {
      services.AddSingleton<IDispatchedEventBus, DispatchedEventBus>();
      return services;
    }
  }
}
