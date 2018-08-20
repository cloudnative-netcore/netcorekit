using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NetCoreKit.Domain;
using Newtonsoft.Json;

namespace NetCoreKit.Infrastructure.AspNetCore.Middlewares
{
  public class ErrorHandlerMiddleware
  {
    private readonly RequestDelegate _next;

    public ErrorHandlerMiddleware(RequestDelegate next)
    {
      _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
      try
      {
        await _next(context);
      }
      catch (Exception exception)
      {
        await HandleErrorAsync(context, exception);
      }
    }

    private static Task HandleErrorAsync(HttpContext context, Exception exception)
    {
      const string errorCode = "error";
      const HttpStatusCode statusCode = HttpStatusCode.BadRequest;
      var message = exception.Message;
      switch (exception)
      {
        case DbUpdateException e:
          message = e.InnerException?.Message;
          break;
        case CoreException e:
          break;
      }

      var response = new
      {
        code = errorCode,
        message
      };

      var payload = JsonConvert.SerializeObject(response);
      context.Response.ContentType = "application/json";
      context.Response.StatusCode = (int)statusCode;

      return context.Response.WriteAsync(payload);
    }
  }
}
