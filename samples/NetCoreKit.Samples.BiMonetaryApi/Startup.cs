using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NetCoreKit.Infrastructure.AspNetCore.Miniservice;

namespace NetCoreKit.Samples.BiMonetaryApi
{
  public class Startup
  {
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddMongoMiniService();
    }

    public void Configure(IApplicationBuilder app)
    {
      app.UseMiniService();
    }
  }
}
