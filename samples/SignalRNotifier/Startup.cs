using System;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetCoreKit.Infrastructure.Bus;
using NetCoreKit.Infrastructure.Bus.Kafka;
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
        typeof(DomainDomainEventBus)
        /*typeof(ProjectCreated)*/);

      Mapper.Initialize(cfg => cfg.AddProfiles(typeof(Startup)));

      services.AddKafkaEventBus();
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
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
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
      services.Configure<KafkaOptions>(config);
      return resolver;
    }
  }
}
