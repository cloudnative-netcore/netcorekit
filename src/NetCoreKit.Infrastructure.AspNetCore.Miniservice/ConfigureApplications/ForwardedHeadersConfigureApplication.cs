using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace NetCoreKit.Infrastructure.AspNetCore.Miniservice.ConfigureApplications
{
  public class ForwardedHeadersConfigureApplication : IConfigureApplication
  {
    public int Priority { get; } = 500;
    public void Configure(IApplicationBuilder app)
    {
      var env = app.ApplicationServices.GetRequiredService<IHostingEnvironment>();

      if (!env.IsDevelopment())
      {
        app.UseForwardedHeaders();
      }
    }
  }
}