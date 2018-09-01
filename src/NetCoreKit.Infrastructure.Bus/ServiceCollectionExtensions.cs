using Microsoft.Extensions.DependencyInjection;
using NetCoreKit.Domain;

namespace NetCoreKit.Infrastructure.Bus
{
  public static class ServiceCollectionExtensions
  {
    public static IServiceCollection AddInMemoryEventBus(this IServiceCollection services)
    {
      services.AddSingleton<IEventBus, InMemoryEventBus>();
      return services;
    }
  }
}
