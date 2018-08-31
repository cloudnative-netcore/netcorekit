using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NetCoreKit.Domain;
using NetCoreKit.Utils.Attributes;

namespace NetCoreKit.Infrastructure.AspNetCore.CleanArch
{
  public abstract class TxRequestHandlerBase<TRequest, TResponse> : ITxEventHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
  {
    protected TxRequestHandlerBase(IUnitOfWorkAsync uow, IQueryRepositoryFactory queryRepositoryFactory)
    {
      QueryFactory = queryRepositoryFactory;
      UnitOfWork = uow;
    }

    public IQueryRepositoryFactory QueryFactory { get; }

    public IUnitOfWorkAsync UnitOfWork { get; }

    public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
  }
}
