using System;
using System.Threading;
using System.Threading.Tasks;
using NetCoreKit.Domain;
using NetCoreKit.Infrastructure.AspNetCore.CleanArch;
using NetCoreKit.Infrastructure.EfCore.Extensions;

namespace NetCoreKit.Samples.TodoAPI.v1.UseCases.DeleteTodo
{
	public class RequestHandler : TxRequestHandlerBase<DeleteTodoRequest, DeleteTodoResponse>
	{
		public RequestHandler(IUnitOfWorkAsync uow, IQueryRepositoryFactory queryRepositoryFactory)
			: base(uow, queryRepositoryFactory)
		{
		}

		public override async Task<DeleteTodoResponse> Handle(DeleteTodoRequest request,
			CancellationToken cancellationToken)
		{
			var commandRepository = UnitOfWork.Repository<Domain.Todo>();
			var queryRepository = QueryRepositoryFactory.QueryEfRepository<Domain.Todo>();

			var todo = await queryRepository.GetByIdAsync(request.Id);
			if (todo == null)
			{
				throw new Exception($"Could not find item #{request.Id}.");
			}

			await commandRepository.DeleteAsync(todo);

		  await UnitOfWork.SaveChangesAsync(cancellationToken);
      return new DeleteTodoResponse {Result = todo.Id};
		}
	}
}
