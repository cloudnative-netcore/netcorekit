using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NetCoreKit.Domain;

namespace NetCoreKit.Infrastructure.AspNetCore.CleanArch
{
  /// <summary>
  /// Source at https://jimmybogard.com/life-beyond-distributed-transactions-an-apostates-implementation-dispatching-example
  /// </summary>
  /// <typeparam name="TRequest"></typeparam>
  /// <typeparam name="TResponse"></typeparam>
  public class UnitOfWorkBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest,TResponse>
  {
    private readonly IUnitOfWorkAsync _unitOfWork;

    public UnitOfWorkBehavior(IUnitOfWorkAsync unitOfWork)
    {
      _unitOfWork = unitOfWork;
    }

    public async Task<TResponse> Handle(
      TRequest request,
      CancellationToken cancellationToken,
      RequestHandlerDelegate<TResponse> next)
    {
      using (_unitOfWork)
      {
        var response = await next();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return response;
      }
    }
  }
}
