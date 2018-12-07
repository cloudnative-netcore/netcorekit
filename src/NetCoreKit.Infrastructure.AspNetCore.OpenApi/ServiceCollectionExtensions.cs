using System.Linq;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCoreKit.Infrastructure.AspNetCore.Configuration;
using NetCoreKit.Infrastructure.Features;
using Swashbuckle.AspNetCore.Swagger;

namespace NetCoreKit.Infrastructure.AspNetCore.OpenApi
{
  public static class ServiceCollectionExtensions
  {
    public static IServiceCollection AddOpenApiCore(this IServiceCollection services, IConfiguration config, IFeature feature)
    {
      services.Configure<OpenApiOptions>(config.GetSection("Features:OpenApi"));

      services.AddSwaggerGen(c =>
      {
        var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

        c.DescribeAllEnumsAsStrings();

        foreach (var description in provider.ApiVersionDescriptions)
          c.SwaggerDoc(
            description.GroupName,
            CreateInfoForApiVersion(config, description));

        // c.IncludeXmlComments (XmlCommentsFilePath);

        if (feature.IsEnabled("AuthN"))
          c.AddSecurityDefinition("oauth2", new OAuth2Scheme
          {
            Type = "oauth2",
            Flow = "implicit",
            AuthorizationUrl = $"{config.GetExternalAuthUri()}/connect/authorize",
            TokenUrl = $"{config.GetExternalAuthUri()}/connect/token",
            Scopes = config.GetScopes()
          });

        c.EnableAnnotations();

        if (feature.IsEnabled("AuthN"))
          c.OperationFilter<SecurityRequirementsOperationFilter>();

        c.OperationFilter<DefaultValuesOperationFilter>();
        c.SchemaFilter<SwaggerExcludeSchemaFilter>();
        c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
      });

      return services;
    }

    private static Info CreateInfoForApiVersion(IConfiguration config, ApiVersionDescription description)
    {
      var info = new Info
      {
        Title = $"{config.GetValue("Features:OpenApi:ApiInfo:Title", "API")} {description.ApiVersion}",
        Version = description.ApiVersion.ToString(),
        Description = config.GetValue("Features:OpenApi:ApiInfo:Description",
          "An application with Swagger, Swashbuckle, and API versioning."),
        Contact = new Contact
        {
          Name = config.GetValue("Features:OpenApi:ApiInfo:ContactName", "Vietnam Devs"),
          Email = config.GetValue("Features:OpenApi:ApiInfo:ContactEmail", "vietnam.devs.group@gmail.com")
        },
        TermsOfService = config.GetValue("Features:OpenApi:ApiInfo:TermOfService", "Shareware"),
        License = new License
        {
          Name = config.GetValue("Features:OpenApi:ApiInfo:LicenseName", "MIT"),
          Url = config.GetValue("Features:OpenApi:ApiInfo:LicenseUrl", "https://opensource.org/licenses/MIT")
        }
      };

      if (description.IsDeprecated)
      {
        info.Description += " This API version has been deprecated.";
      }

      return info;
    }
  }
}
