using System;
using System.Threading;
using System.Threading.Tasks;
using NetCoreKit.Domain;
using NetCoreKit.Infrastructure.AspNetCore.CleanArch;
using NetCoreKit.Infrastructure.EfCore.Extensions;
using NetCoreKit.Samples.TodoAPI.Extensions;

namespace NetCoreKit.Samples.TodoAPI.v1.UseCases.UpdateTodo
{
	public class RequestHandler : TxRequestHandlerBase<UpdateTodoRequest, UpdateTodoResponse>
	{
		public RequestHandler(IUnitOfWorkAsync uow, IQueryRepositoryFactory queryRepositoryFactory)
			: base(uow, queryRepositoryFactory)
		{
		}

		public override async Task<UpdateTodoResponse> Handle(UpdateTodoRequest request,
			CancellationToken cancellationToken)
		{
			var commandRepository = UnitOfWork.Repository<Domain.Todo>();
			var queryRepository = QueryRepositoryFactory.QueryEfRepository<Domain.Todo>();

			var todo = await queryRepository.GetByIdAsync(request.Id);
			if (todo == null)
			{
				throw new Exception($"Could not find item #{request.Id}.");
			}

		  todo.ChangeTitle(request.Title)
		    .ChangeOrder(request.Order ?? 1);

		  if (request.Completed.HasValue && request.Completed.Value == true)
		  {
		    todo.ChangeToCompleted();
		  }

			var updated = await commandRepository.UpdateAsync(todo);
		  await UnitOfWork.SaveChangesAsync(cancellationToken);

			return new UpdateTodoResponse {Result = updated.ToDto()};
		}
	}
}
