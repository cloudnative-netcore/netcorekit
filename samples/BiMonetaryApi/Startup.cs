using BeatPulse.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NetCoreKit.Infrastructure.AspNetCore.Miniservice;

namespace NetCoreKit.Samples.BiMonetaryApi
{
  public class Startup
  {
    public void ConfigureServices(IServiceCollection services)
    {
      services
        .AddMongoMiniService()
        .AddBeatPulse()
        .AddBeatPulseUI();
    }

    public void Configure(IApplicationBuilder app)
    {
      app
        .UseBeatPulse(options =>
        {
          options.ConfigurePath(path: "healthz") //default hc
            .ConfigureTimeout(milliseconds: 1500) // default -1 infinitely
            .ConfigureDetailedOutput(detailedOutput: true, includeExceptionMessages: true); //default (true,false)
        })
        .UseBeatPulseUI()
        .UseMiniService();
    }
  }
}
