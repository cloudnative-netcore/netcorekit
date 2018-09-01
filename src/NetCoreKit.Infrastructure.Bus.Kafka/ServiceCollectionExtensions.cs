using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NetCoreKit.Domain;

namespace NetCoreKit.Infrastructure.Bus.Kafka
{
  public static class ServiceCollectionExtensions
  {
    public static IServiceCollection AddKafkaEventBus(this IServiceCollection services)
    {
      services.Replace(ServiceDescriptor.Singleton<IEventBus, EventBus>());

      return services;
    }
  }
}
