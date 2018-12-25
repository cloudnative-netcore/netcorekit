using System;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetCoreKit.Infrastructure.AspNetCore.Configuration;
using NetCoreKit.Infrastructure.Bus;
using NetCoreKit.Infrastructure.Bus.Redis;
using NetCoreKit.Samples.SignalRNotifier.Services.Hubs;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace NetCoreKit.Samples.SignalRNotifier
{
    public class Startup
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMediatR(
                typeof(Startup),
                typeof(DomainEventBus));

            Mapper.Initialize(cfg => cfg.AddProfiles(typeof(Startup)));

            services.AddRedisBus();
            services.AddSignalR();
            services.AddSingleton<IHostedService, ProjectHostService>();

            services.AddCors(options => options.AddPolicy("AllRequestPolicy", builder =>
            {
                builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowAnyOrigin()
                    .AllowCredentials()
                    .SetIsOriginAllowedToAllowWildcardSubdomains();
            }));

            return services.BuildServiceProvider();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var config = app.ApplicationServices.GetRequiredService<IConfiguration>();
            var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();

            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            var basePath = config.GetBasePath();

            if (!string.IsNullOrEmpty(basePath))
            {
                var logger = loggerFactory.CreateLogger("init");
                logger.LogInformation($"Using PATH BASE '{basePath}'");
                app.UsePathBase(basePath);
            }

            app.UseCors("AllRequestPolicy");

            app.UseSignalR(routes => { routes.MapHub<ProjectHub>("/project"); });
        }
    }
}
