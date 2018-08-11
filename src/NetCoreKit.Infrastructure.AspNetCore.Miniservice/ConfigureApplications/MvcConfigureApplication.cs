using Microsoft.AspNetCore.Builder;

namespace NetCoreKit.Infrastructure.AspNetCore.Miniservice.ConfigureApplications
{
  public class MvcConfigureApplication : IConfigureApplication
  {
    public int Priority { get; } = 800;
    public void Configure(IApplicationBuilder app)
    {
      app.UseMvc();
    }
  }
}