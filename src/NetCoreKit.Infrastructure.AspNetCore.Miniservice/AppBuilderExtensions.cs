using System.Diagnostics;
using System.Reflection;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetCoreKit.Infrastructure.AspNetCore.Configuration;
using NetCoreKit.Infrastructure.AspNetCore.Extensions;
using NetCoreKit.Infrastructure.AspNetCore.Middlewares;
using NetCoreKit.Infrastructure.Features;
using StackExchange.Profiling;
using static NetCoreKit.Utils.Helpers.IdHelper;

namespace NetCoreKit.Infrastructure.AspNetCore.Miniservice
{
  public static class AppBuilderExtensions
  {
    public static IApplicationBuilder UseMiniService(this IApplicationBuilder app)
    {
      var config = app.ApplicationServices.GetRequiredService<IConfiguration>();
      var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
      var env = app.ApplicationServices.GetRequiredService<IHostingEnvironment>();
      var feature = app.ApplicationServices.GetRequiredService<IFeature>();

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

        if (feature.IsEnabled("OpenApi:Profiler"))
          app.UseMiniProfiler();
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

      if (feature.IsEnabled("OpenApi:Profiler"))
        app.UseMiddleware<MiniProfilerMiddleware>();

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
      if (!env.IsDevelopment())
        app.UseForwardedHeaders();

      // #6
      app.UseCors("CorsPolicy");

      // #7
      if (feature.IsEnabled("AuthN"))
        app.UseAuthentication();

      // #8
      app.UseMvc();

      // #9
      basePath = config.GetBasePath();
      var currentHostUri = config.GetExternalCurrentHostUri();

      if (feature.IsEnabled("OpenApi"))
        app.UseSwagger();

      if (feature.IsEnabled("OpenApi:UI"))
        app.UseSwaggerUI(
          c =>
          {
            var provider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();

            // build a swagger endpoint for each discovered API version
            foreach (var description in provider.ApiVersionDescriptions)
              c.SwaggerEndpoint(
                $"{basePath}swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant());

            if (feature.IsEnabled("AuthN"))
            {
              c.OAuthClientId("swagger_id");
              c.OAuthClientSecret("secret".Sha256());
              c.OAuthAppName("swagger_app");
              c.OAuth2RedirectUrl($"{currentHostUri}/swagger/oauth2-redirect.html");
            }

            if (feature.IsEnabled("OpenApi:Profiler"))
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
