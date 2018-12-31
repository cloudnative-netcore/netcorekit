using System.Threading.Tasks;
using MediatR;
using NetCoreKit.Domain;
using NetCoreKit.Infrastructure.Mappers;

namespace NetCoreKit.Infrastructure.Bus
{
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IMediator _mediator;

        public DomainEventDispatcher(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Dispatch(IEvent @event)
        {
            await _mediator.Publish(@event.MapTo<IEvent, INotification>());
        }

        public void Dispose()
        {
        }
    }
}
