using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCoreKit.Utils.Attributes;
using NetCoreKit.Utils.Extensions;

namespace NetCoreKit.Infrastructure.AspNetCore.Miniservice.ConfigureServices
{
  public class CleanArchConfigureService : IBasicConfigureServices
  {
    public int Priority { get; } = 300;

    public void Configure(IServiceCollection services)
    {
      var svcProvider = services.BuildServiceProvider();
      var serviceParams = svcProvider.GetRequiredService<ServiceParams>();
      var config = svcProvider.GetRequiredService<IConfiguration>();

      if (!(serviceParams["assemblies"] is HashSet<Assembly> assemblies)) return;

      var ass = config.GetValue<string>("QualifiedAssemblyPattern").LoadAssemblyWithPattern();

      services.AddScoped<ServiceFactory>(p => p.GetService);
      services.AddScoped<IMediator, Mediator>();
      //services.AddMediatR(ass.ToArray());

      /*services.AddScoped<ServiceFactory>(p => p.GetService);
      services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
      services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>));
      services.AddScoped<IMediator, Mediator>();*/

      /*services.Scan(
        scanner => scanner
          .FromAssemblies(ass.ToArray())
          .AddClasses(x => x.WithAttribute<AutoScanAwarenessAttribute>())
          .AsImplementedInterfaces());*/
    }
  }
}
