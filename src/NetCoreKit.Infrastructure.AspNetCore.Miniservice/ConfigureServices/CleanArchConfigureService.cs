using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NetCoreKit.Utils.Attributes;

namespace NetCoreKit.Infrastructure.AspNetCore.Miniservice.ConfigureServices
{
  public class CleanArchConfigureService : IBasicConfigureServices
  {
    public int Priority { get; } = 300;

    public void Configure(IServiceCollection services)
    {
      var svcProvider = services.BuildServiceProvider();
      var serviceParams = svcProvider.GetRequiredService<ServiceParams>();

      if (!(serviceParams["assemblies"] is HashSet<Assembly> assemblies)) return;

      services.AddMediatR(assemblies.ToArray());
      services.Scan(
        scanner => scanner
          .FromAssemblies(assemblies.ToArray())
          .AddClasses(x => x.WithAttribute<AutoScanAwarenessAttribute>())
          .AsImplementedInterfaces());
    }
  }
}
