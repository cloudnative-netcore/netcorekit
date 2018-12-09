using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NetCoreKit.Template.MongoDb;

namespace NetCoreKit.Samples.BiMonetaryApi
{
  public class Startup
  {
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddMongoDbTemplate();
    }

    public void Configure(IApplicationBuilder app)
    {
      app.UseMongoDbTemplate();
    }
  }
}
