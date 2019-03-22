using System;
using System.IO;
using System.Linq;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetCoreKit.Domain;
using NetCoreKit.Infrastructure;
using NetCoreKit.Infrastructure.EfCore;
using NetCoreKit.Infrastructure.EfCore.Db;
using NetCoreKit.Infrastructure.Features;

namespace NetCoreKit.Template.Grpc.EfCore
{
    public static class HostBuilderExtensions
    {
        public static IHost ConfigureDefaultSettings<TDbContext>(this HostBuilder hostBuilder,
            string[] args,
            Action<IServiceCollection> preDbWorkHook = null,
            Action<IServiceCollection> moreRegisterAction = null)
            where TDbContext : DbContext
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

                        if (feature.IsEnabled("EfCore"))
                        {
                            if (feature.IsEnabled("Mongo"))
                                throw new Exception("Should turn off MongoDb settings.");

                            preDbWorkHook?.Invoke(services);

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

                        services.AddSingleton<IDomainEventDispatcher, MemoryDomainEventDispatcher>();

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
