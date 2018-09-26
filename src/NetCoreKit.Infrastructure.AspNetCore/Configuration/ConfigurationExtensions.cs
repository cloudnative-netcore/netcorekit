using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace NetCoreKit.Infrastructure.AspNetCore.Configuration
{
  public static class ConfigurationExtensions
  {
    public static string GetHostUri(this IConfiguration config, IHostingEnvironment env, string groupName)
    {
      return env.IsDevelopment() ? config.GetExternalHostUri(groupName) : config.GetInternalHostUri(groupName);
    }

    public static string GetInternalHostUri(this IConfiguration config, string groupName)
    {
      var group = config
          .GetSection("Hosts")
          ?.GetSection("Internals")
          ?.GetSection(groupName);

      var serviceDnsName = group.GetValue<string>("ServiceName").ToUpperInvariant();
      var serviceHost = $"{Environment.GetEnvironmentVariable($"{serviceDnsName}_SERVICE_HOST")}";
      var servicePort = $"{Environment.GetEnvironmentVariable($"{serviceDnsName}_SERVICE_PORT")}";
      var basePath = $"{group.GetValue("BasePath", string.Empty)}";

      return $"http://{serviceHost}:{servicePort}{basePath}";
    }

    public static string GetExternalHostUri(this IConfiguration config, string groupName)
    {
      return config
          .GetSection("Hosts")
          ?.GetSection("Externals")
          ?.GetSection(groupName)
          ?.GetValue<string>("Uri");
    }

    public static string GetBasePath(this IConfiguration config)
    {
      return config
        .GetSection("Hosts")
        ?.GetValue<string>("BasePath");
    }

    public static string GetExternalCurrentHostUri(this IConfiguration config)
    {
      return config
        .GetSection("Hosts")
        ?.GetSection("Externals")
        ?.GetValue<string>("CurrentUri");
    }
  }
}
