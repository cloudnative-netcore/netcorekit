using System.Linq;
using System.Reflection;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace NetCoreKit.Infrastructure.AspNetCore.OpenApi
{
  public class SwaggerExcludeSchemaFilter : ISchemaFilter
  {
    public void Apply(Schema schema, SchemaFilterContext context)
    {
      if (schema?.Properties == null) return;

      var excludedProperties = context.SystemType.GetProperties()
        .Where(t => t.GetCustomAttribute<SwaggerExcludeAttribute>() != null);
      foreach (var excludedProperty in excludedProperties)
        if (schema.Properties.ContainsKey(excludedProperty.Name))
          schema.Properties.Remove(excludedProperty.Name);
    }
  }
}
