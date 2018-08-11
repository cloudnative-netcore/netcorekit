using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace NetCoreKit.Infrastructure.AspNetCore.Miniservice.ConfigureServices
{
  public class MvcConfigureService : IBasicConfigureServices
  {
    public int Priority { get; } = 500;

    public void Configure(IServiceCollection services)
    {
      services
        .AddMvc()
        .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
    }
  }
}
