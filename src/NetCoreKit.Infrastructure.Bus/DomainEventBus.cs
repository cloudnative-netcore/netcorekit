using System.Threading.Tasks;
using MediatR;
using NetCoreKit.Domain;
using NetCoreKit.Infrastructure.Mappers;

namespace NetCoreKit.Infrastructure.Bus
{
    public class DomainEventBus : IDomainEventBus
    {
        private readonly IMediator _mediator;

        public DomainEventBus(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Publish(IEvent @event)
        {
            await _mediator.Publish(@event.MapTo<IEvent, INotification>());
        }

        public void Dispose()
        {
        }
    }
}
