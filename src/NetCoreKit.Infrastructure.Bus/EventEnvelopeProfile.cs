using AutoMapper;
using NetCoreKit.Domain;
using NetCoreKit.Infrastructure.Mappers;

namespace NetCoreKit.Infrastructure.Bus
{
  public class EventEnvelopeProfile : Profile
  {
    public EventEnvelopeProfile()
    {
      this.MapToNotification<EventEnvelope, NotificationEnvelope>();
    }
  }
}
