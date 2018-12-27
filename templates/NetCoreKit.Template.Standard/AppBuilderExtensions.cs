using System.Reflection;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetCoreKit.Infrastructure.AspNetCore.All;
using NetCoreKit.Infrastructure.AspNetCore.Configuration;
using NetCoreKit.Infrastructure.AspNetCore.Middlewares;
using NetCoreKit.Infrastructure.Features;
using StackExchange.Profiling;

namespace NetCoreKit.Template.Standard
{
    public static class AppBuilderExtensions
    {
        public static IApplicationBuilder UseStandardTemplate(this IApplicationBuilder app)
        {
            var config = app.ApplicationServices.GetRequiredService<IConfiguration>();
            var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
            var env = app.ApplicationServices.GetRequiredService<IHostingEnvironment>();
            var feature = app.ApplicationServices.GetRequiredService<IFeature>();

            // #1 Log exception handler
            app.UseMiddleware<LogHandlerMiddleware>();

            // #2 Default response cache
            app.UseResponseCaching();

            // #3 configure Exception handling
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();

                if (feature.IsEnabled("OpenApi:Profiler"))
                    app.UseMiniProfiler();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UseExceptionHandlerCore();
            app.UseMiddleware<ErrorHandlerMiddleware>();

            // #4 BeatPulse healthcheck and BeatPulse UI 
            app.UseBeatPulse(options =>
            {
                options.ConfigurePath("healthz") //default hc
                    .ConfigureTimeout(1500) // default -1 infinitely
                    .ConfigureDetailedOutput(true, true); //default (true,false)
            });

            // #5 Miniprofiler on API
            if (feature.IsEnabled("OpenApi:Profiler"))
                app.UseMiddleware<MiniProfilerMiddleware>();

            // #6 liveness endpoint
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            app.Map("/liveness", lapp => lapp.Run(async ctx => ctx.Response.StatusCode = 200));
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

            // #7 Re-configure the base path
            var basePath = config.GetBasePath();
            if (!string.IsNullOrEmpty(basePath))
            {
                var logger = loggerFactory.CreateLogger("init");
                logger.LogInformation($"Using PATH BASE '{basePath}'");
                app.UsePathBase(basePath);
            }

            // #8 ForwardHeaders
            if (!env.IsDevelopment())
                app.UseForwardedHeaders();

            // #9 Cors
            app.UseCors("AllRequestPolicy");

            // #10 AuthN
            if (feature.IsEnabled("AuthN"))
                app.UseAuthentication();

            // #11 Mvc
            app.UseMvc();

            // #12 Open API
            if (feature.IsEnabled("OpenApi"))
                app.UseSwagger();

            if (feature.IsEnabled("OpenApi:OpenApiUI"))
                app.UseSwaggerUI(
                    c =>
                    {
                        var provider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();
                        foreach (var description in provider.ApiVersionDescriptions)
                            c.SwaggerEndpoint(
                                $"{basePath}swagger/{description.GroupName}/swagger.json",
                                description.GroupName.ToUpperInvariant());

                        if (feature.IsEnabled("AuthN"))
                        {
                            c.OAuthClientId("swagger_id");
                            c.OAuthClientSecret("secret".Sha256());
                            c.OAuthAppName("swagger_app");
                            c.OAuth2RedirectUrl($"{config.GetExternalCurrentHostUri()}/swagger/oauth2-redirect.html");
                        }

                        if (feature.IsEnabled("OpenApi:Profiler"))
                            c.IndexStream = () =>
                                typeof(Infrastructure.AspNetCore.All.ServiceCollectionExtensions)
                                    .GetTypeInfo()
                                    .Assembly
                                    .GetManifestResourceStream(
                                        "NetCoreKit.Infrastructure.AspNetCore.All.Swagger.index.html");
                    });

            return app;
        }
    }
}
