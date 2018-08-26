using System;
using System.Collections.Generic;
using System.Linq;

namespace NetCoreKit.Infrastructure.AspNetCore.Miniservice
{
  public static class ServiceParamsExtensions
  {
    public static ServiceParams ExtendServiceParams(this ServiceParams serviceParams,
      IEnumerable<KeyValuePair<string, object>> extends = null)
    {
      if (extends == null || !extends.Any()) return serviceParams;
      var conParams = serviceParams.Concat(extends);

      // to avoid a side-effect then we don't manipulate directly ServiceParams in the input
      var result = new ServiceParams();
      foreach (var param in conParams) result.Add(param.Key, param.Value);

      return result;
    }

    public static Dictionary<string, string> GetScopes(this ServiceParams serviceParams)
    {
      return serviceParams["scopes"] as Dictionary<string, string>;
    }

    public static Dictionary<string, string> GetClaims(this ServiceParams serviceParams)
    {
      if (serviceParams.TryGetValue("claimToScopeMap", out var claimToScopeMap))
        return claimToScopeMap as Dictionary<string, string>;

      throw new Exception("Couldn't parse and get [claimToScopeMap].");
    }

    public static string GetAudience(this ServiceParams serviceParams)
    {
      return serviceParams["audience"].ToString();
    }
  }
}

