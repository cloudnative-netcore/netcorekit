using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCoreKit.Domain;
using NetCoreKit.Infrastructure.AspNetCore.CleanArch;
using NetCoreKit.Infrastructure.AspNetCore.OpenApi;
using NetCoreKit.Infrastructure.EfCore;
using NetCoreKit.Infrastructure.EfCore.Db;
using NetCoreKit.Infrastructure.Features;

namespace NetCoreKit.Infrastructure.AspNetCore.Miniservice
{
  public static partial class ServiceCollectionExtensions
  {
    public static IServiceCollection AddEfCoreMiniService<TDbContext>(
      this IServiceCollection services,
      Action<IServiceCollection> preDbWorkHook = null,
      Action<IServiceCollection, IServiceProvider> postDbWorkHook = null)
      where TDbContext : DbContext
    {
      services.AddFeatureToggle();

      using (var scope = services.BuildServiceProvider().GetService<IServiceScopeFactory>().CreateScope())
      {
        var svcProvider = scope.ServiceProvider;
        var config = svcProvider.GetRequiredService<IConfiguration>();
        var env = svcProvider.GetRequiredService<IHostingEnvironment>();
        var feature = svcProvider.GetRequiredService<IFeature>();

        // let registering the database providers or others from the outside
        preDbWorkHook?.Invoke(services);

        // #1
        if (feature.IsEnabled("EfCore"))
        {
          if (feature.IsEnabled("Mongo")) throw new Exception("Please turn off MongoDb settings.");

          services.AddDbContextPool<TDbContext>((sp, o) =>
          {
            var extendOptionsBuilder = sp.GetRequiredService<IExtendDbContextOptionsBuilder>();
            var connStringFactory = sp.GetRequiredService<IDatabaseConnectionStringFactory>();
            extendOptionsBuilder.Extend(o, connStringFactory,
              config.LoadApplicationAssemblies().FirstOrDefault()?.GetName().Name);
          });

          services.AddScoped<DbContext>(resolver => resolver.GetService<TDbContext>());
          services.AddGenericRepository();
        }

        // let outside inject more logic (like more healthcheck endpoints...)
        postDbWorkHook?.Invoke(services, svcProvider);

        // RestClient out of the box
        services.AddRestClientCore();

        // DomainEventBus for handling the published event from the domain object
        services.AddSingleton<IDomainEventBus, MemoryDomainEventBus>();

        // #3
        if (feature.IsEnabled("CleanArch"))
          services.AddCleanArch(config.LoadFullAssemblies());

        services.AddCacheCore();

        // #4
        if (feature.IsEnabled("ApiVersion"))
          services.AddApiVersionCore(config);

        // #5
        services.AddMvcCore(config);

        // #6
        services.AddDetailExceptionCore();

        // #7
        if (feature.IsEnabled("AuthN"))
          services.AddAuthNCore(config, env);

        // #8
        if (feature.IsEnabled("OpenApi"))
          services.AddOpenApiCore(config, feature);

        // #9
        services.AddCorsCore();

        // #10
        services.AddHeaderForwardCore(env);

        if (feature.IsEnabled("OpenApi:Profiler"))
          services.AddApiProfilerCore();
      }

      return services;
    }
  }
}
