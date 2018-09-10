using AutoMapper;
using NetCoreKit.Domain;

namespace NetCoreKit.Infrastructure.Mappers
{
  public static class AutoMapperProfileExtension
  {
    public static Profile MapMySelf<TEvent>(this Profile profile) where TEvent : IEvent
    {
      profile.CreateMap(typeof(TEvent), typeof(TEvent)).ReverseMap();
      return profile;
    }

    public static TResponse MapTo<TRequest, TResponse>(this TRequest request)
    {
      return Mapper.Map<TResponse>(request);
    }
  }
}
