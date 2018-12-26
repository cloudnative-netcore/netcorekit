using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using NetCoreKit.Infrastructure.AspNetCore.Rest;
using NetCoreKit.Utils.Helpers;

namespace NetCoreKit.Infrastructure.AspNetCore.All
{
    public static class AppBuilderExtensions
    {
        public static IApplicationBuilder UseExceptionHandlerCore(this IApplicationBuilder app)
        {
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
                            Instance = $"urn:myorganization:error:{IdHelper.GenerateId()}"
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

            return app;
        }
    }
}
