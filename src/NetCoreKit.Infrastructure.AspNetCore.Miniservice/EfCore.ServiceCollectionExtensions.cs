using System;
using System.Linq;
using BeatPulse.Core;
using BeatPulse.UI;
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
    public static IServiceCollection AddEfCoreMiniService<TDbContext>(this IServiceCollection services,
      Action<IServiceCollection> preDbWorkHook = null,
      Action<IServiceCollection, IServiceProvider> postDbWorkHook = null,
      Action<BeatPulseContext> beatPulseCtx = null)
      where TDbContext : DbContext
    {
      services.AddFeatureToggle();

      using (var scope = services.BuildServiceProvider().GetService<IServiceScopeFactory>().CreateScope())
      {
        var svcProvider = scope.ServiceProvider;
        var config = svcProvider.GetRequiredService<IConfiguration>();
        var env = svcProvider.GetRequiredService<IHostingEnvironment>();
        var feature = svcProvider.GetRequiredService<IFeature>();

        preDbWorkHook?.Invoke(services);

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

        postDbWorkHook?.Invoke(services, svcProvider);

        services.AddRestClientCore();

        services.AddSingleton<IDomainEventBus, MemoryDomainEventBus>();

        if (feature.IsEnabled("CleanArch"))
          services.AddCleanArch(config.LoadFullAssemblies());

        services.AddCacheCore();

        if (feature.IsEnabled("ApiVersion"))
          services.AddApiVersionCore(config);

        services.AddMvcCore(config);

        services.AddDetailExceptionCore();

        if (feature.IsEnabled("AuthN"))
          services.AddAuthNCore(config, env);

        if (feature.IsEnabled("OpenApi"))
          services.AddOpenApiCore(config, feature);

        services.AddCorsCore();

        services.AddHeaderForwardCore(env);

        if (feature.IsEnabled("OpenApi:Profiler"))
          services.AddApiProfilerCore();

        services.AddBeatPulse(beatPulseCtx);

        if (feature.IsEnabled("HealthUI"))
          services.AddBeatPulseUI();
      }

      return services;
    }
  }
}
