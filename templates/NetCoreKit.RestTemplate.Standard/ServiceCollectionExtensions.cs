using System;
using BeatPulse.Core;
using MessagePack.AspNetCoreMvcFormatter;
using MessagePack.Resolvers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCoreKit.Domain;
using NetCoreKit.Infrastructure;
using NetCoreKit.Infrastructure.AspNetCore.All;
using NetCoreKit.Infrastructure.AspNetCore.CleanArch;
using NetCoreKit.Infrastructure.AspNetCore.OpenApi;
using NetCoreKit.Infrastructure.Features;

namespace NetCoreKit.RestTemplate.Standard
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStandardTemplate(this IServiceCollection services,
            Action<IServiceCollection, IServiceProvider> preHook = null,
            Action<BeatPulseContext> beatPulseCtx = null)
        {
            services.AddFeatureToggle();

            using (var scope = services.BuildServiceProvider().GetService<IServiceScopeFactory>().CreateScope())
            {
                var svcProvider = scope.ServiceProvider;
                var config = svcProvider.GetRequiredService<IConfiguration>();
                var env = svcProvider.GetRequiredService<IHostingEnvironment>();
                var feature = svcProvider.GetRequiredService<IFeature>();

                preHook?.Invoke(services, svcProvider);

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
