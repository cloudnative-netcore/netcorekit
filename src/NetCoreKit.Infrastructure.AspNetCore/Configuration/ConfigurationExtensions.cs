using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using NetCoreKit.Infrastructure.AspNetCore.Authz;

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

    public static Dictionary<string, string> GetScopes(this IConfiguration config)
    {
      return GetAuthNOptions(config).Scopes;
    }

    public static Dictionary<string, string> GetClaims(this IConfiguration config)
    {
      return GetAuthNOptions(config).ClaimToScopeMap;
    }

    public static string GetAudience(this IConfiguration config)
    {
      return GetAuthNOptions(config).Audience;
    }

    public static AuthNOptions GetAuthNOptions(this IConfiguration config)
    {
      var options = new AuthNOptions();
      config.GetSection("Features:AuthN").Bind(options);
      return options;
    }

    public static string GetAuthUri(this IConfiguration config, IHostingEnvironment env)
    {
      return config.GetHostUri(env, "Auth");
    }

    public static string GetExternalAuthUri(this IConfiguration config)
    {
      return config.GetExternalHostUri("Auth");
    }
  }
}
