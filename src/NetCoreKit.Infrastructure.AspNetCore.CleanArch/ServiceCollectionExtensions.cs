using System.Collections.Generic;
using System.Reflection;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace NetCoreKit.Infrastructure.AspNetCore.CleanArch
{
  public static class ServiceCollectionExtensions
  {
    public static IServiceCollection AddCleanArch(this IServiceCollection services, IEnumerable<Assembly> registeredAssemblies)
    {
      services.AddScoped(typeof(IPipelineBehavior<,>), typeof(UnitOfWorkBehavior<,>));
      Mapper.Initialize(cfg => cfg.AddProfiles(registeredAssemblies));
      services.AddMediatR(registeredAssemblies);

      return services;
    }
  }
}
