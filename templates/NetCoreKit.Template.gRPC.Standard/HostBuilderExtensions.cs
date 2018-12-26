using System;
using System.IO;
using System.Linq;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetCoreKit.Infrastructure;

namespace NetCoreKit.Template.gRPC.Standard
{
    public static class HostBuilderExtensions
    {
        public static IHost ConfigureDefaultSettings(this HostBuilder hostBuilder,
            string[] args,
            Action<IServiceCollection> moreRegisterAction)
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

                    using (var scope = services.BuildServiceProvider().GetService<IServiceScopeFactory>().CreateScope())
                    {
                        var svcProvider = scope.ServiceProvider;
                        var config = svcProvider.GetRequiredService<IConfiguration>();

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
