using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using MediatR;

namespace NetCoreKit.Infrastructure.AspNetCore.CleanArch
{
    public static class EventAggregatorExtensions
    {
        public static IObservable<dynamic> SendStream<TRequest, TResponse>(this IMediator mediator,
            TRequest request, Func<TResponse, dynamic> mapTo = null,
            CancellationToken token = default(CancellationToken))
            where TRequest : IRequest<TResponse>
            where TResponse : class
        {
            return mediator.Send(request, token)
                .ToObservable()
                .Select(x => x.PresentFor(mapTo));
        }
    }
}
