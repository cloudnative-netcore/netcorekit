using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NetCoreKit.Domain;
using NetCoreKit.Infrastructure.AspNetCore.Miniservice;
using NetCoreKit.Infrastructure.Bus;
using NetCoreKit.Infrastructure.Bus.Kafka;
using NetCoreKit.Infrastructure.EfCore;
using NetCoreKit.Samples.TodoAPI.Domain;
using NetCoreKit.Samples.TodoAPI.Infrastructure.Db;
using NetCoreKit.Samples.TodoAPI.Infrastructure.Gateways;

namespace NetCoreKit.Samples.TodoAPI
{
  public class Startup
  {
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddMiniService<TodoListDbContext>(
        svc =>
        {
          // svc.AddEfCoreMySqlDb();
          svc.AddEfSqlLiteDb();
          svc.AddExternalSystemHealthChecks();
          svc.AddInMemoryEventBus();
          // svc.AddKafkaEventBus();
        },
        (svc, _) => { svc.AddScoped<IUserGateway, UserGateway>(); }
      );

      /*services.BuildServiceProvider()
        .GetService<IEventBus>()
        .Subscribe<ProjectCreated>();*/
    }

    public void Configure(IApplicationBuilder app)
    {
      app.UseMiniService();
    }
  }
}
