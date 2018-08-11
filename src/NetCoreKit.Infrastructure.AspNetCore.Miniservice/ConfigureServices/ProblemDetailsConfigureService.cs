using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NetCoreKit.Infrastructure.AspNetCore.Validation;

namespace NetCoreKit.Infrastructure.AspNetCore.Miniservice.ConfigureServices
{
  public class ProblemDetailsConfigureService : IBasicConfigureServices
  {
    public int Priority { get; } = 600;

    public void Configure(IServiceCollection services)
    {
      services.Configure<ApiBehaviorOptions>(options =>
      {
        options.InvalidModelStateResponseFactory = ctx => new ValidationProblemDetailsResult();
      });
    }
  }
}
