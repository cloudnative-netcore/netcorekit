using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NetCoreKit.Infrastructure.AspNetCore.Miniservice;
using NetCoreKit.Infrastructure.EfCore.MySql;
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
          svc.AddEfCoreMySqlDb();
          svc.AddExternalSystemHealthChecks();
        },
        (svc, _) => { svc.AddScoped<IUserGateway, UserGateway>(); }
      );
    }

    public void Configure(IApplicationBuilder app)
    {
      app.UseMiniService();
    }
  }
}
