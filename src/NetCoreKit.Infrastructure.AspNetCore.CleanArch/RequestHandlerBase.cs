using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NetCoreKit.Domain;

namespace NetCoreKit.Infrastructure.AspNetCore.CleanArch
{
  public abstract class RequestHandlerBase<TRequest, TResponse> : IEventHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
  {
    protected RequestHandlerBase(IQueryRepositoryFactory queryRepositoryFactory)
    {
      QueryRepositoryFactory = queryRepositoryFactory;
    }

    public IQueryRepositoryFactory QueryRepositoryFactory { get; }

    public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
  }
}
