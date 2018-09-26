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
using NetCoreKit.Infrastructure.AspNetCore.Extensions;
using NetCoreKit.Infrastructure.Bus;
using NetCoreKit.Infrastructure.Bus.Kafka;
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
        typeof(DomainEventBus)
        /*typeof(ProjectCreated)*/);

      Mapper.Initialize(cfg => cfg.AddProfiles(typeof(Startup)));

      // services.AddKafkaEventBus();
      services.AddRedisBus();
      services.AddSignalR();
      services.AddSingleton<IHostedService, ProjectHostService>();

      services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
      {
        builder
          .AllowAnyMethod()
          .AllowAnyHeader()
          .AllowAnyOrigin()
          .AllowCredentials()
          .SetIsOriginAllowedToAllowWildcardSubdomains();
      }));

      return BuildServiceProvider(services);
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      var config = app.ApplicationServices.GetRequiredService<IConfiguration>();
      var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();

      loggerFactory.AddConsole(config.GetSection("Logging"));
      loggerFactory.AddDebug();

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      var basePath = config.GetBasePath();

      if (!string.IsNullOrEmpty(basePath))
      {
        var logger = loggerFactory.CreateLogger("init");
        logger.LogInformation($"Using PATH BASE '{basePath}'");
        app.UsePathBase(basePath);
      }

      app.UseCors("CorsPolicy");

      app.UseSignalR(routes =>
      {
        routes.MapHub<ProjectHub>("/project");
      });
    }

    private static IServiceProvider BuildServiceProvider(IServiceCollection services)
    {
      var resolver = services.BuildServiceProvider();
      var config = resolver.GetRequiredService<IConfiguration>();
      var env = resolver.GetRequiredService<IHostingEnvironment>();
      var kafkaOptions = config.GetSection("EventBus");
      //if (env.IsDevelopment())
      {
        services.Configure<KafkaOptions>(o => { o.Brokers = kafkaOptions.GetValue<string>("Brokers"); });
      }
      /*else
      {
        var serviceName = kafkaOptions
          .GetValue("ServiceName", "kafka")
          .Replace("-", "_")
          .ToUpperInvariant();

        var ip = Environment.GetEnvironmentVariable($"{serviceName}_SERVICE_HOST");
        var port = Environment.GetEnvironmentVariable($"{serviceName}_SERVICE_PORT");

        services.Configure<KafkaOptions>(o => { o.Brokers = $"{ip}:{port}"; });
      }*/

      return resolver;
    }
  }
}
