using System.Threading;
using System.Threading.Tasks;
using NetCoreKit.Domain;
using NetCoreKit.Infrastructure.AspNetCore.CleanArch;
using NetCoreKit.Samples.TodoAPI.Domain;
using NetCoreKit.Samples.TodoAPI.Extensions;

namespace NetCoreKit.Samples.TodoAPI.v1.UseCases.AddTodo
{
	public class RequestHandler : TxRequestHandlerBase<AddTodoRequest, AddTodoResponse>
	{
		public RequestHandler(IUnitOfWorkAsync uow, IQueryRepositoryFactory queryRepositoryFactory)
			: base(uow, queryRepositoryFactory)
		{
		}

		public override async Task<AddTodoResponse> Handle(AddTodoRequest request, CancellationToken cancellationToken)
		{
			var todoRepository = UnitOfWork.Repository<Todo>();

			var todo = Todo.Load(request.Title);
			var result = await todoRepository.AddAsync(todo);

		  await UnitOfWork.SaveChangesAsync(cancellationToken);
			return new AddTodoResponse {Result = result.ToDto()};
		}
	}
}
