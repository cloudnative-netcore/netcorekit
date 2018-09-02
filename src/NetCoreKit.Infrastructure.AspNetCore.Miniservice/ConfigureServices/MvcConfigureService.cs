using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCoreKit.Utils.Extensions;

namespace NetCoreKit.Infrastructure.AspNetCore.Miniservice.ConfigureServices
{
  public class MvcConfigureService : IBasicConfigureServices
  {
    public int Priority { get; } = 500;

    public void Configure(IServiceCollection services)
    {
      var svcProvider = services.BuildServiceProvider();
      var serviceParams = svcProvider.GetRequiredService<ServiceParams>();
      var config = svcProvider.GetRequiredService<IConfiguration>();

      //var mvcBuilder = services.AddMvc();
      //if (serviceParams["assemblies"] is HashSet<Assembly> assemblies && assemblies.Count > 0)
      /*var assemblies = config.GetValue<string>("QualifiedAssemblyPattern").LoadAssemblyWithPattern();
      foreach (var assembly in assemblies) mvcBuilder = mvcBuilder.AddApplicationPart(assembly);*/
      //mvcBuilder.SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
    }
  }
}
