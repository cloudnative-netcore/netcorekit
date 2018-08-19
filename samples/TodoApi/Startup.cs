using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NetCoreKit.Infrastructure.AspNetCore.Miniservice;
using NetCoreKit.Samples.TodoAPI.Infrastructure.Db;

namespace NetCoreKit.Samples.TodoAPI
{
  public class Startup
  {
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddMiniService<TodoDbContext>(
        new[] {typeof(Startup)},
        svc =>
        {
          // svc.AddEfCoreSqlServerDb();
          // svc.AddEfCoreMySqlDb();
          svc.AddExternalSystemHealthChecks();
        }
      );
    }

    public void Configure(IApplicationBuilder app)
    {
      app.UseMiniService();
    }
  }
}
