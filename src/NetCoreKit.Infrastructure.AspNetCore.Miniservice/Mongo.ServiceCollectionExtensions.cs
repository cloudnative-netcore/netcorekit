using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCoreKit.Domain;
using NetCoreKit.Infrastructure.AspNetCore.CleanArch;
using NetCoreKit.Infrastructure.AspNetCore.OpenApi;
using NetCoreKit.Infrastructure.AspNetCore.Rest;
using NetCoreKit.Infrastructure.AspNetCore.Validation;
using NetCoreKit.Infrastructure.Mongo;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;

namespace NetCoreKit.Infrastructure.AspNetCore.Miniservice
{
  public static partial class ServiceCollectionExtensions
  {
    public static IServiceCollection AddMongoMiniService(
      this IServiceCollection services,
      Action<IServiceCollection> preScopeAction = null,
      Action<IServiceCollection, IServiceProvider> afterDbScopeAction = null)
    {
      using (var scope = services.BuildServiceProvider().GetService<IServiceScopeFactory>().CreateScope())
      {
        var svcProvider = scope.ServiceProvider;
        var config = svcProvider.GetRequiredService<IConfiguration>();
        var env = svcProvider.GetRequiredService<IHostingEnvironment>();

        // let registering the database providers or others from the outside
        preScopeAction?.Invoke(services);

        // #1
        if (config.GetValue("Mongo:Enabled", false))
        {
          if (config.GetValue<bool>("EfCore:Enabled"))
            throw new Exception("Please turn off EfCore settings.");
          services.AddMongoDb();
        }

        // let outside inject more logic (like more healthcheck endpoints...)
        afterDbScopeAction?.Invoke(services, svcProvider);

        // #2
        if (config.GetValue("RestClient:Enabled", false))
        {
          services.AddHttpContextAccessor();
          services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
          services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
          services.AddSingleton<IUrlHelper>(fac => new UrlHelper(fac.GetService<IActionContextAccessor>().ActionContext));
          services.AddHttpPolly<RestClient>();
        }

        // #3
        services.AddSingleton<IDomainEventBus, MemoryDomainEventBus>();

        if (config.GetValue("CleanArchitecture:Enabled", false))
        {
          services.AddCleanArch(config.LoadFullAssemblies());
        }

        services.AddMemoryCache();
        services.AddResponseCaching();

        // #4
        if (config.GetValue("ApiVersion:Enabled", false))
        {
          services.AddRouting(o => o.LowercaseUrls = true);
          services
            .AddMvcCore()
            .AddVersionedApiExplorer(
              o =>
              {
                o.GroupNameFormat = "'v'VVV";
                o.SubstituteApiVersionInUrl = true;
              })
            .AddJsonFormatters(o => o.ContractResolver = new CamelCasePropertyNamesContractResolver())
            .AddDataAnnotations();

          services.AddApiVersioning(o =>
          {
            o.ReportApiVersions = true;
            o.AssumeDefaultVersionWhenUnspecified = true;
            o.DefaultApiVersion = ParseApiVersion(config.GetValue<string>("API_VERSION"));
          });
        }

        // #5
        var mvcBuilder = services.AddMvc();
        if (config.LoadFullAssemblies() != null && config.LoadFullAssemblies().Any())
          foreach (var assembly in config.LoadFullAssemblies())
            mvcBuilder = mvcBuilder.AddApplicationPart(assembly);
        mvcBuilder.SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

        // #6
        services.Configure<ApiBehaviorOptions>(options =>
        {
          options.InvalidModelStateResponseFactory = ctx => new ValidationProblemDetailsResult();
        });

        // #7
        if (config.GetValue("AuthN:Enabled", false))
        {
          JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

          services
            .AddAuthentication(options =>
            {
              options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
              options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
              options.Authority = GetAuthUri(config, env);
              options.RequireHttpsMetadata = false;
              options.Audience = config.GetAudience();
            });

          services.AddAuthorization(c =>
          {
            foreach (var claimToScope in config.GetClaims())
              c.AddPolicy(claimToScope.Key, p => p.RequireClaim("scope", claimToScope.Value));
          });
        }

        // #8
        if (config.GetValue("OpenApi:Enabled", false))
        {
          if (config.GetSection("OpenApi") == null)
            throw new Exception("Please add OpenApi configuration or disabled OpenAPI.");

          services.Configure<OpenApiOptions>(config.GetSection("OpenApi"));

          services.AddSwaggerGen(c =>
          {
            var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

            c.DescribeAllEnumsAsStrings();

            foreach (var description in provider.ApiVersionDescriptions)
              c.SwaggerDoc(
                description.GroupName,
                CreateInfoForApiVersion(config, description));

            // c.IncludeXmlComments (XmlCommentsFilePath);

            if (config.GetValue("AuthN:Enabled", false))
              c.AddSecurityDefinition("oauth2", new OAuth2Scheme
              {
                Type = "oauth2",
                Flow = "implicit",
                AuthorizationUrl = $"{GetExternalAuthUri(config)}/connect/authorize",
                TokenUrl = $"{GetExternalAuthUri(config)}/connect/token",
                Scopes = config.GetScopes()
              });

            c.EnableAnnotations();

            if (config.GetValue("AuthN:Enabled", false))
              c.OperationFilter<SecurityRequirementsOperationFilter>();

            c.OperationFilter<DefaultValuesOperationFilter>();
            c.SchemaFilter<SwaggerExcludeSchemaFilter>();
            c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
          });
        }

        // #9
        services.AddCors(options =>
        {
          options.AddPolicy("CorsPolicy",
            policy => policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
        });

        // #10
        if (!env.IsDevelopment())
          services.Configure<ForwardedHeadersOptions>(options =>
          {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
          });

        if (config.GetValue("OpenApi:Profiler:Enabled", false))
        {
          services.AddMiniProfiler(options =>
            options.RouteBasePath = "/profiler"
          );
        }
      }

      return services;
    }
  }
}
