using System;
using AutoMapper;

namespace NetCoreKit.Infrastructure.Mappers
{
  public static class AutoMapperProfileExtension
  {
    public static Profile MapMySelf(this Profile profile, Type type)
    {
      profile.CreateMap(type, type).ReverseMap();
      return profile;
    }

    public static TResponse MapTo<TRequest, TResponse>(this TRequest request)
    {
      return Mapper.Map<TResponse>(request);
    }
  }
}
