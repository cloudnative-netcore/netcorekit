using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NetCoreKit.Domain;

namespace NetCoreKit.Infrastructure.Bus
{
  public static class ServiceCollectionExtensions
  {
    public static IServiceCollection AddDomainEventBus(this IServiceCollection services)
    {
      services.Replace(ServiceDescriptor.Singleton<IDomainEventBus, DomainEventBus>());
      return services;
    }
  }
}
