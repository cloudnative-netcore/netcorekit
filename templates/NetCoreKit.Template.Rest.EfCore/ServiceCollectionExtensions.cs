using System;
using System.Linq;
using BeatPulse.Core;
using MessagePack.AspNetCoreMvcFormatter;
using MessagePack.Resolvers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCoreKit.Domain;
using NetCoreKit.Infrastructure;
using NetCoreKit.Infrastructure.AspNetCore.All;
using NetCoreKit.Infrastructure.AspNetCore.CleanArch;
using NetCoreKit.Infrastructure.AspNetCore.OpenApi;
using NetCoreKit.Infrastructure.EfCore;
using NetCoreKit.Infrastructure.EfCore.Db;
using NetCoreKit.Infrastructure.Features;

namespace NetCoreKit.Template.Rest.EfCore
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEfCoreTemplate<TDbContext>(this IServiceCollection services,
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
                    if (feature.IsEnabled("Mongo")) throw new Exception("Should turn MongoDb feature off.");

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

                services.AddSingleton<IDomainEventDispatcher, MemoryDomainEventDispatcher>();

                services.AddAutoMapperCore(config.LoadFullAssemblies());
                services.AddMediatRCore(config.LoadFullAssemblies());

                if (feature.IsEnabled("CleanArch"))
                    services.AddCleanArch();

                services.AddCacheCore();

                if (feature.IsEnabled("ApiVersion"))
                    services.AddApiVersionCore(config);

                var mvcBuilder = services.AddMvcCore(config);

                if (feature.IsEnabled("MessagePack"))
                    mvcBuilder.AddMvcOptions(option =>
                    {
                        option.OutputFormatters.Clear();
                        option.OutputFormatters.Add(
                            new MessagePackOutputFormatter(ContractlessStandardResolver.Instance));
                        option.InputFormatters.Clear();
                        option.InputFormatters.Add(
                            new MessagePackInputFormatter(ContractlessStandardResolver.Instance));
                    });

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

                if (feature.IsEnabled("ResponseCompression"))
                    services.AddResponseCompression();
            }

            return services;
        }
    }
}
