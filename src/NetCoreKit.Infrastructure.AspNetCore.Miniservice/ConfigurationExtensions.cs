using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using NetCoreKit.Infrastructure.AspNetCore.Miniservice.Options;

namespace NetCoreKit.Infrastructure.AspNetCore.Miniservice
{
  public static class ConfigurationExtensions
  {
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

    private static AuthNOptions GetAuthNOptions(IConfiguration config)
    {
      var options = new AuthNOptions();
      config.GetSection("Features:AuthN").Bind(options);
      return options;
    }
  }
}

