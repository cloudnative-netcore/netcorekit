using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using NetCoreKit.Domain;

namespace NetCoreKit.Infrastructure.Bus
{
  public class InMemoryEventBus : IEventBus
  {
    private readonly IMediator _mediator;

    public InMemoryEventBus(IMediator mediator)
    {
      _mediator = mediator;
    }

    public async Task Publish(IEvent @event, params string[] topics)
    {
      await _mediator.Publish(Mapper.Map<INotification>(@event));
    }

    public async Task Subscribe(params string[] topics)
    {
      // because this is in-memory so that we don't have anything to do here
      await Task.FromResult(true);
    }

    public async Task SubscribeAsync(params string[] topics)
    {
      // because this is in-memory so that we don't have anything to do here
      await Task.FromResult(true);
    }
  }
}
