using System;
using System.Collections.Generic;
using System.Linq;
using NetCoreKit.Utils.Extensions;

namespace NetCoreKit.Infrastructure.AspNetCore.Miniservice
{
  public static class ServiceParamsExtensions
  {
    public static ServiceParams GetServiceParams(this IServiceProvider sp,
      IEnumerable<Type> assemblyTypes, IEnumerable<KeyValuePair<string, object>> extends = null)
    {
      var svcParams = new ServiceParams
      {
        {
          "assemblies", assemblyTypes.Append(typeof(ServiceCollectionExtensions)).GetAssembliesByTypes()
        }
      };

      if (extends == null || !extends.Any()) return svcParams;
      var conParams = svcParams.Concat(extends);

      // to avoid a side-effect then we don't manipulate directly ServiceParams in the input
      var result = new ServiceParams();
      foreach (var param in conParams)
      {
        result.Add(param.Key, param.Value);
      }

      return result;
    }
  }
}
