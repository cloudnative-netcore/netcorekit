using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NetCoreKit.Infrastructure.AspNetCore.All.Controllers
{
  [Route("")]
  [ApiVersionNeutral]
  [ApiExplorerSettings(IgnoreApi = true)]
  public class SysInfoController : Controller
  {
    private readonly IConfiguration _config;

    public SysInfoController(IConfiguration config)
    {
      _config = config;
    }

    [HttpGet("/sysinfo")]
    public IActionResult Index()
    {
      // Source: http://michaco.net/blog/EnvironmentVariablesAndConfigurationInASPNETCoreApps
      var basePath = PlatformServices.Default.Application.ApplicationBasePath;

      // the app's name and version
      var appName = PlatformServices.Default.Application.ApplicationName;
      var appVersion = PlatformServices.Default.Application.ApplicationVersion;

      // object with some dotnet runtime version information
      var runtimeFramework = PlatformServices.Default.Application.RuntimeFramework;

      // envs
      var envs = new Dictionary<string, object>();

      foreach (var env in _config.GetChildren()) envs.Add(env.Key, env.Key);

      dynamic model = new JObject();

      model.OSArchitecture = !RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
        ? (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
          ? "Linux or OSX"
          : "Others")
        : "Windows";

      model.OSDescription = RuntimeInformation.OSDescription;

      model.ProcessArchitecture = RuntimeInformation.ProcessArchitecture == Architecture.Arm
        ? "Arm"
        : (RuntimeInformation.ProcessArchitecture == Architecture.Arm64
          ? "Arm64"
          : (RuntimeInformation.ProcessArchitecture == Architecture.X64
            ? "x64"
            : (RuntimeInformation.ProcessArchitecture == Architecture.X86
              ? "x86"
              : "Others")));

      model.BasePath = basePath;
      model.AppName = appName;
      model.AppVersion = appVersion;
      model.RuntimeFramework = runtimeFramework.ToString();
      model.FrameworkDescription = RuntimeInformation.FrameworkDescription;
      model.Envs = JsonConvert.SerializeObject(envs, new JsonSerializerSettings
      {
        Formatting = Formatting.Indented
      });

      return Content(model.ToString());
    }
  }
}
