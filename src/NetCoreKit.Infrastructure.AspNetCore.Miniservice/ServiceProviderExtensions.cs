using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace NetCoreKit.Infrastructure.AspNetCore.Miniservice
{
  internal static class ServiceProviderExtensions
  {
    public static IOrderedEnumerable<TConfigureService> GetServicesByPriority<TConfigureService>(
      this IServiceProvider svcProvider)
      where TConfigureService : IPriorityConfigure
    {
      return svcProvider
        .GetServices<TConfigureService>()
        .OrderBy(x => x.Priority);
    }
  }
}
