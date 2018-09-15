using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NetCoreKit.Domain;
using NetCoreKit.Infrastructure.Mappers;

namespace NetCoreKit.Infrastructure.Bus
{
  public class EnvelopeEventHandler : INotificationHandler<NotificationEnvelope>
  {
    private readonly IMediator _mediator;

    public EnvelopeEventHandler(IMediator mediator)
    {
      _mediator = mediator;
    }

    public async Task Handle(NotificationEnvelope notification, CancellationToken cancellationToken)
    {
      await _mediator.Publish(notification.Event.MapTo<IEvent, INotification>(), cancellationToken);
    }
  }
}
