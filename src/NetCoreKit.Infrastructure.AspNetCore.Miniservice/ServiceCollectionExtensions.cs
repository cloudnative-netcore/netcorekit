using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetCoreKit.Infrastructure.EfCore;

namespace NetCoreKit.Infrastructure.AspNetCore.Miniservice
{
  public static class ServiceCollectionExtensions
  {
    public static IServiceCollection AddMiniService<TDbContext>(
      this IServiceCollection services,
      IEnumerable<Type> registeredAssemblyTypes,
      Action<IServiceCollection> databaseRegistrationAction = null,
      Action<IServiceCollection> postRegistrationAction = null)
      where TDbContext : DbContext
    {
      if (registeredAssemblyTypes == null || !registeredAssemblyTypes.Any())
        throw new Exception("Should have at least one assembly in Startup file.");

      services.AddScoped(sp => sp.GetServiceParams(registeredAssemblyTypes));
      services.ScanAndRegisterServices<IPriorityConfigure>();

      using (var scope = services.BuildServiceProvider().CreateScope())
      {
        var svcProvider = scope.ServiceProvider;

        services.AddEfCore();
        databaseRegistrationAction?.Invoke(services);

        var dbConfigureSvcs = svcProvider.GetServicesByPriority<IDbConfigureServices>();
        foreach (var configureSvc in dbConfigureSvcs)
          configureSvc.Configure<TDbContext>(services);

        var configureSvcs = svcProvider.GetServicesByPriority<IBasicConfigureServices>();
        foreach (var configureSvc in configureSvcs)
          configureSvc.Configure(services);

        postRegistrationAction?.Invoke(services);
      }

      return services;
    }

    public static IServiceCollection AddExternalSystemHealthChecks(this IServiceCollection services,
      Func<IServiceProvider, IEnumerable<IExternalSystem>> extendExternalSystem = null)
    {
      if (extendExternalSystem == null)
        return services;
      services.AddSingleton(p => extendExternalSystem(p).Append(p.GetService<DbHealthCheckAndMigration>()));

      return services;
    }

    public static IApplicationBuilder UseMiniService(this IApplicationBuilder app)
    {
      using (var scope = app.ApplicationServices.CreateScope())
      {
        var appSvc = scope.ServiceProvider;
        var configureApps = appSvc.GetServicesByPriority<IConfigureApplication>();
        foreach (var configureApp in configureApps)
          configureApp.Configure(app);
      }

      return app;
    }

    private static IServiceCollection ScanAndRegisterServices<TConfigureService>(this IServiceCollection services)
    {
      using (var scope = services.BuildServiceProvider().CreateScope())
      {
        var svcProvider = scope.ServiceProvider;
        var serviceParams = svcProvider.GetRequiredService<ServiceParams>();
        var assemblies = serviceParams["assemblies"] as HashSet<Assembly>;

        return services.Scan(
          scanner => scanner
            .FromAssemblies(assemblies)
            .AddClasses(x => x.AssignableTo<TConfigureService>())
            .AsImplementedInterfaces()
            .WithScopedLifetime());
      }
    }
  }
}
