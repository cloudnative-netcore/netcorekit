using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetCoreKit.Infrastructure.AspNetCore.CleanArch;
using NetCoreKit.Infrastructure.AspNetCore.Configuration;
using NetCoreKit.Infrastructure.AspNetCore.Extensions;
using NetCoreKit.Infrastructure.AspNetCore.Middlewares;
using NetCoreKit.Infrastructure.AspNetCore.Miniservice.ExternalSystems;
using NetCoreKit.Infrastructure.AspNetCore.OpenApi;
using NetCoreKit.Infrastructure.AspNetCore.Rest;
using NetCoreKit.Infrastructure.AspNetCore.Validation;
using NetCoreKit.Infrastructure.Bus;
using NetCoreKit.Infrastructure.EfCore;
using NetCoreKit.Infrastructure.EfCore.Db;
using Newtonsoft.Json.Serialization;
using StackExchange.Profiling;
using Swashbuckle.AspNetCore.Swagger;
using static NetCoreKit.Utils.Helpers.IdHelper;

namespace NetCoreKit.Infrastructure.AspNetCore.Miniservice
{
  public static class ServiceCollectionExtensions
  {
    public static IServiceCollection AddMiniService<TDbContext>(
      this IServiceCollection services,
      Action<IServiceCollection> preScopeAction = null,
      Action<IServiceCollection, IServiceProvider> afterDbScopeAction = null,
      Func<IEnumerable<KeyValuePair<string, object>>> extendServiceParamsFunc = null)
      where TDbContext : DbContext
    {
      services.AddScoped(sp => new ServiceParams().ExtendServiceParams(extendServiceParamsFunc?.Invoke()));

      using (var scope = services.BuildServiceProvider().GetService<IServiceScopeFactory>().CreateScope())
      {
        var svcProvider = scope.ServiceProvider;
        var config = svcProvider.GetRequiredService<IConfiguration>();
        var env = svcProvider.GetRequiredService<IHostingEnvironment>();
        var serviceParams = svcProvider.GetRequiredService<ServiceParams>();

        // let registering the database providers or others from the outside
        preScopeAction?.Invoke(services);
        // services.AddScoped<DbHealthCheckAndMigration>();

        // #1
        services.AddDbContextPool<TDbContext>((sp, o) =>
        {
          var extendOptionsBuilder = sp.GetRequiredService<IExtendDbContextOptionsBuilder>();
          var connStringFactory = sp.GetRequiredService<IDatabaseConnectionStringFactory>();
          extendOptionsBuilder.Extend(o, connStringFactory,
            config.LoadApplicationAssemblies().FirstOrDefault()?.GetName().Name);
        });

        services.AddScoped<DbContext>(resolver => resolver.GetService<TDbContext>());
        services.AddGenericRepository();

        // let outside inject more logic (like more healthcheck endpoints...)
        afterDbScopeAction?.Invoke(services, svcProvider);

        // #2
        services.AddHttpContextAccessor();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
        services.AddSingleton<IUrlHelper>(
          fac => new UrlHelper(fac.GetService<IActionContextAccessor>().ActionContext));
        services.AddHttpPolly<RestClient>();

        // #3
        services.AddDomainEventBus();
        services.AddCleanArch(config.LoadFullAssemblies());

        services.AddMemoryCache();
        services.AddResponseCaching();

        // #4
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
        if (config.GetValue("EnableAuthN", false))
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
              options.Audience = serviceParams.GetAudience();
            });

          services.AddAuthorization(c =>
          {
            foreach (var claimToScope in serviceParams.GetClaims())
              c.AddPolicy(claimToScope.Key, p => p.RequireClaim("scope", claimToScope.Value));
          });
        }

        // #8
        if (config.GetSection("OpenApi") == null)
          throw new Exception("Please add OpenApi configuration or disabled OpenAPI.");

        if (config.GetValue("OpenApi:Enabled", false))
        {
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

            if (config.GetValue("EnableAuthN", false))
              c.AddSecurityDefinition("oauth2", new OAuth2Scheme
              {
                Type = "oauth2",
                Flow = "implicit",
                AuthorizationUrl = $"{GetExternalAuthUri(config)}/connect/authorize",
                TokenUrl = $"{GetExternalAuthUri(config)}/connect/token",
                Scopes = serviceParams.GetScopes()
              });

            c.EnableAnnotations();

            if (config.GetValue("EnableAuthN", false))
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

        if (config.GetValue("OpenApi:EnabledProfiler", false))
        {
          services.AddMiniProfiler(options =>
            options.RouteBasePath = "/profiler"
          );
        }
      }

      return services;
    }

    public static IServiceCollection AddExternalSystemHealthChecks(this IServiceCollection services,
      Func<IServiceProvider, IEnumerable<IExternalSystem>> extendExternalSystem = null)
    {
      return services.AddScoped(p =>
      {
        var results = extendExternalSystem?.Invoke(p);
        return results == null
          ? new List<IExternalSystem> {p.GetService<DbHealthCheckAndMigration>()}
          : results.Append(p.GetService<DbHealthCheckAndMigration>());
      });
    }

    public static ApiVersion ParseApiVersion(string serviceVersion)
    {
      if (string.IsNullOrEmpty(serviceVersion))
      {
        throw new Exception("[CS] ServiceVersion is null or empty.");
      }

      const string pattern = @"(.)|(-)";
      var results = Regex.Split(serviceVersion, pattern)
        .Where(x => x != string.Empty && x != "." && x != "-")
        .ToArray();

      if (results == null || results.Count() < 2)
      {
        throw new Exception("[CS] Could not parse ServiceVersion.");
      }

      if (results.Count() > 2)
      {
        return new ApiVersion(
          Convert.ToInt32(results[0]),
          Convert.ToInt32(results[1]),
          results[2]);
      }

      return new ApiVersion(
        Convert.ToInt32(results[0]),
        Convert.ToInt32(results[1]));
    }

    public static string GetAuthUri(IConfiguration config, IHostingEnvironment env)
    {
      return config.GetHostUri(env, "Auth");
    }

    public static string GetExternalAuthUri(IConfiguration config)
    {
      return config.GetExternalHostUri("Auth");
    }

    public static Info CreateInfoForApiVersion(IConfiguration config, ApiVersionDescription description)
    {
      var info = new Info()
      {
        Title = $"{config.GetValue("OpenApi:Title", "API")} {description.ApiVersion}",
        Version = description.ApiVersion.ToString(),
        Description = config.GetValue("OpenApi:Description", "An application with Swagger, Swashbuckle, and API versioning."),
        Contact = new Contact()
        {
          Name = config.GetValue("OpenApi:ContactName", "Vietnam Devs"),
          Email = config.GetValue("OpenApi:ContactEmail", "vietnam.devs.group@gmail.com")
        },
        TermsOfService = config.GetValue("OpenApi:TermOfService", "Shareware"),
        License = new License()
        {
          Name = config.GetValue("OpenApi:LicenseName", "MIT"),
          Url = config.GetValue("OpenApi:LicenseUrl", "https://opensource.org/licenses/MIT")
        }
      };

      if (description.IsDeprecated)
      {
        info.Description += " This API version has been deprecated.";
      }

      return info;
    }

    public static IApplicationBuilder UseMiniService(this IApplicationBuilder app)
    {
      var config = app.ApplicationServices.GetRequiredService<IConfiguration>();
      var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
      var env = app.ApplicationServices.GetRequiredService<IHostingEnvironment>();

      // #1
      loggerFactory.AddConsole(config.GetSection("Logging"));
      loggerFactory.AddDebug();

      app.UseMiddleware<LogHandlerMiddleware>();

      app.UseResponseCaching();

      // #2
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseDatabaseErrorPage();

        if (config.GetValue("OpenApi:EnabledProfiler", false))
        {
          app.UseMiniProfiler();
        }
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
      }

      app.UseExceptionHandler(errorApp =>
      {
#pragma warning disable CS1998
        errorApp.Run(async context =>
          {
            var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
            var exception = errorFeature.Error;

            // the IsTrusted() extension method doesn't exist and
            // you should implement your own as you may want to interpret it differently
            // i.e. based on the current principal
            var problemDetails = new ProblemDetails
            {
              Instance = $"urn:myorganization:error:{GenerateId()}"
            };

            if (exception is BadHttpRequestException badHttpRequestException)
            {
              problemDetails.Title = "Invalid request";
              problemDetails.Status = (int)typeof(BadHttpRequestException)
                .GetProperty("StatusCode", BindingFlags.NonPublic | BindingFlags.Instance)
                  ?.GetValue(badHttpRequestException);
              problemDetails.Detail = badHttpRequestException.Message;
            }
            else
            {
              problemDetails.Title = "An unexpected error occurred!";
              problemDetails.Status = 500;
              problemDetails.Detail = exception.Demystify().ToString();
            }

            // TODO: log the exception etc..
            // ...

            context.Response.StatusCode = problemDetails.Status.Value;
            context.Response.WriteJson(problemDetails, "application/problem+json");
          }
#pragma warning restore CS1998
        );
      });

      app.UseMiddleware<ErrorHandlerMiddleware>();

      if (config.GetValue("OpenApi:EnabledProfiler", false))
      {
        app.UseMiddleware<MiniProfilerMiddleware>();
      }

      // #3
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
      app.Map("/liveness", lapp => lapp.Run(async ctx => ctx.Response.StatusCode = 200));
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

      // #4
      var basePath = config.GetBasePath();

      if (!string.IsNullOrEmpty(basePath))
      {
        var logger = loggerFactory.CreateLogger("init");
        logger.LogInformation($"Using PATH BASE '{basePath}'");
        app.UsePathBase(basePath);
      }

      // #5
      if (!env.IsDevelopment()) app.UseForwardedHeaders();

      // #6
      app.UseCors("CorsPolicy");

      // #7
      if (config.GetValue("EnableAuthN", false)) app.UseAuthentication();

      // #8
      app.UseMvc();

      // #9
      basePath = config.GetBasePath();
      var currentHostUri = config.GetExternalCurrentHostUri();

      if (config.GetValue("OpenApi:Enabled", false)) app.UseSwagger();

      if (config.GetValue("OpenApi:EnabledUI", false))
        app.UseSwaggerUI(
          c =>
          {
            var provider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();

            // build a swagger endpoint for each discovered API version
            foreach (var description in provider.ApiVersionDescriptions)
              c.SwaggerEndpoint(
                $"{basePath}swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant());

            if (config.GetValue("EnableAuthN", false))
            {
              c.OAuthClientId("swagger_id");
              c.OAuthClientSecret("secret".Sha256());
              c.OAuthAppName("swagger_app");
              c.OAuth2RedirectUrl($"{currentHostUri}/swagger/oauth2-redirect.html");
            }

            if (config.GetValue("OpenApi:EnabledProfiler", false))
            {
              c.IndexStream = () =>
                typeof(ServiceCollectionExtensions)
                  .GetTypeInfo()
                  .Assembly
                  .GetManifestResourceStream("NetCoreKit.Infrastructure.AspNetCore.Miniservice.Swagger.index.html");
            }
          });

      return app;
    }
  }
}
