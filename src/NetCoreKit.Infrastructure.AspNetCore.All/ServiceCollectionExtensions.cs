using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCoreKit.Domain;
using NetCoreKit.Infrastructure.AspNetCore.Configuration;
using NetCoreKit.Infrastructure.AspNetCore.Rest;
using NetCoreKit.Infrastructure.AspNetCore.Validation;
using Newtonsoft.Json.Serialization;

namespace NetCoreKit.Infrastructure.AspNetCore.All
{
  public static class ServiceCollectionExtensions
  {
    public static IServiceCollection AddRestClientCore(this IServiceCollection services)
    {
      services.AddHttpContextAccessor();
      services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
      services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
      services.AddSingleton<IUrlHelper>(fac => new UrlHelper(fac.GetService<IActionContextAccessor>().ActionContext));
      services.AddHttpPolly<RestClient>();

      return services;
    }

    public static IServiceCollection AddCacheCore(this IServiceCollection services)
    {
      services.AddMemoryCache();
      services.AddResponseCaching();

      return services;
    }

    public static IServiceCollection AddApiVersionCore(this IServiceCollection services, IConfiguration config)
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

      return services;
    }

    public static IServiceCollection AddMvcCore(this IServiceCollection services, IConfiguration config)
    {
      var mvcBuilder = services.AddMvc();
      if (config.LoadFullAssemblies() != null && config.LoadFullAssemblies().Any())
        foreach (var assembly in config.LoadFullAssemblies())
          mvcBuilder = mvcBuilder.AddApplicationPart(assembly);
      mvcBuilder.SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

      return services;
    }

    public static IServiceCollection AddDetailExceptionCore(this IServiceCollection services)
    {
      services.Configure<ApiBehaviorOptions>(options =>
      {
        options.InvalidModelStateResponseFactory = ctx => new ValidationProblemDetailsResult();
      });

      return services;
    }

    public static IServiceCollection AddAuthNCore(this IServiceCollection services, IConfiguration config, IHostingEnvironment env)
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
          options.Authority = config.GetAuthUri(env);
          options.RequireHttpsMetadata = false;
          options.Audience = config.GetAudience();
        });

      services.AddAuthorization(c =>
      {
        foreach (var claimToScope in config.GetClaims())
          c.AddPolicy(claimToScope.Key, p => p.RequireClaim("scope", claimToScope.Value));
      });

      return services;
    }

    public static IServiceCollection AddCorsCore(this IServiceCollection services)
    {
      services.AddCors(options =>
      {
        options.AddPolicy("CorsPolicy",
          policy => policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
      });

      return services;
    }

    public static IServiceCollection AddHeaderForwardCore(this IServiceCollection services, IHostingEnvironment env)
    {
      if (!env.IsDevelopment())
        services.Configure<ForwardedHeadersOptions>(options =>
        {
          options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        });

      return services;
    }

    public static IServiceCollection AddApiProfilerCore(this IServiceCollection services)
    {
      services.AddMiniProfiler(options =>
        options.RouteBasePath = "/profiler"
      );

      return services;
    }

    private static ApiVersion ParseApiVersion(string serviceVersion)
    {
      if (string.IsNullOrEmpty(serviceVersion))
      {
        throw new CoreException("[CS] ServiceVersion is null or empty.");
      }

      const string pattern = @"(.)|(-)";
      var results = Regex.Split(serviceVersion, pattern)
        .Where(x => x != string.Empty && x != "." && x != "-")
        .ToArray();

      if (results == null || results.Count() < 2)
      {
        throw new CoreException("[CS] Could not parse ServiceVersion.");
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
  }
}
