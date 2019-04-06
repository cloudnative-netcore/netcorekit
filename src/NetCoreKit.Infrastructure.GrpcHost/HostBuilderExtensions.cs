using System;
using System.IO;
using System.Linq;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetCoreKit.Domain;
using NetCoreKit.Infrastructure.Features;

namespace NetCoreKit.Infrastructure.GrpcHost
{
    public static class HostBuilderExtensions
    {
        public static IHost ConfigureDefaultSettings(this HostBuilder hostBuilder,
            string[] args,
            Action<IServiceCollection> preDbWorkHook = null,
            Action<IServiceCollection> moreRegisterAction = null)
        {
            return hostBuilder
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddJsonFile("hostsettings.json", optional: true);
                    configHost.AddEnvironmentVariables();
                    configHost.AddCommandLine(args);
                })
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.AddJsonFile("appsettings.json", optional: true);
                    configApp.AddJsonFile(
                        $"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json",
                        optional: true);
                    configApp.AddEnvironmentVariables();
                    configApp.AddCommandLine(args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddLogging();
                    services.AddFeatureToggle();

                    using (var scope = services.BuildServiceProvider().GetService<IServiceScopeFactory>().CreateScope())
                    {
                        var svcProvider = scope.ServiceProvider;
                        var config = svcProvider.GetRequiredService<IConfiguration>();
                        var feature = svcProvider.GetRequiredService<IFeature>();

                        services.AddSingleton<IDomainEventDispatcher, MemoryDomainEventDispatcher>();

                        preDbWorkHook?.Invoke(services);

                        Mapper.Initialize(cfg => cfg.AddProfiles(config.LoadFullAssemblies()));
                        services.AddMediatR(config.LoadFullAssemblies().ToArray());

                        moreRegisterAction?.Invoke(services);
                    }
                })
                .ConfigureLogging((hostContext, configLogging) =>
                {
                    configLogging.AddConsole();
                    configLogging.AddDebug();
                })
                .UseConsoleLifetime()
                .Build();
        }
    }
}
