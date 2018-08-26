using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using NetCoreKit.Utils.Extensions;

namespace NetCoreKit.Infrastructure
{
  public static class ConfigurationExtensions
  {
    public static IEnumerable<Assembly> LoadFullAssemblies(this IConfiguration config)
    {
      return config.GetValue<string>("EfCore:FullyQualifiedPrefix").LoadFullAssemblies();
    }

    public static IEnumerable<Assembly> LoadApplicationAssemblies(this IConfiguration config)
    {
      var apps = config.GetValue<string>("EfCore:FullyQualifiedPrefix").LoadAssemblyWithPattern();
      if (apps == null || !apps.Any())
      {
        throw new Exception("Should have at least one application assembly to load.");
      }

      return apps;
    }
  }
}
