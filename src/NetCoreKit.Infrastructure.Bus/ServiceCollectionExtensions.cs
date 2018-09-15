using Microsoft.Extensions.DependencyInjection;
using NetCoreKit.Domain;

namespace NetCoreKit.Infrastructure.Bus
{
  public static class ServiceCollectionExtensions
  {
    public static IServiceCollection AddDomainEventBus(this IServiceCollection services)
    {
      services.AddSingleton<IDomainEventBus, DomainEventBus>();
      return services;
    }
  }
}
