using System;
using System.Collections.Generic;
using System.Linq;
using NetCoreKit.Utils.Extensions;

namespace NetCoreKit.Infrastructure.AspNetCore.Miniservice
{
  public static class ServiceParamsExtensions
  {
    public static ServiceParams GetServiceParams(this IServiceProvider svcProvider,
      IEnumerable<Type> assemblyTypes, IEnumerable<KeyValuePair<string, object>> extends = null)
    {
      var svcParams = new ServiceParams
      {
        {
          "assemblies", assemblyTypes.Append(typeof(MiniServiceExtensions)).GetAssembliesByTypes()
        }
      };

      if (extends == null || !extends.Any()) return svcParams;
      foreach (var extend in extends)
      {
        svcParams.Append(extend);
      }

      return svcParams;
    }
  }
}
