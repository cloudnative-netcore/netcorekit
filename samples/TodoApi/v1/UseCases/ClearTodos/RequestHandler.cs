using System.Threading;
using System.Threading.Tasks;
using NetCoreKit.Domain;
using NetCoreKit.Infrastructure.AspNetCore.CleanArch;
using NetCoreKit.Infrastructure.EfCore.Extensions;

namespace NetCoreKit.Samples.TodoAPI.v1.UseCases.ClearTodos
{
  public class RequestHandler : TxRequestHandlerBase<ClearTodosRequest, ClearTodosResponse>
  {
    public RequestHandler(IUnitOfWorkAsync uow, IQueryRepositoryFactory queryRepositoryFactory)
      : base(uow, queryRepositoryFactory)
    {
    }

    public override async Task<ClearTodosResponse> Handle(ClearTodosRequest request,
      CancellationToken cancellationToken)
    {
      var commandRepository = UnitOfWork.Repository<Domain.Todo>();
      var queryRepository = QueryRepositoryFactory.QueryEfRepository<Domain.Todo>();

      var todos = await queryRepository.ListAsync();
      if (todos == null || todos.Count <= 0) return new ClearTodosResponse();
      //TODO: need to have a ClearAll method in CommandRepository
      foreach (var todo in todos)
      {
        await commandRepository.DeleteAsync(todo);
      }

      await UnitOfWork.SaveChangesAsync(cancellationToken);
      return new ClearTodosResponse();
    }
  }
}
