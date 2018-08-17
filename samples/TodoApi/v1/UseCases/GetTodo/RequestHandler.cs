using System.Threading;
using System.Threading.Tasks;
using NetCoreKit.Domain;
using NetCoreKit.Infrastructure.AspNetCore.CleanArch;
using NetCoreKit.Infrastructure.EfCore.Extensions;
using NetCoreKit.Samples.TodoAPI.Extensions;

namespace NetCoreKit.Samples.TodoAPI.v1.UseCases.GetTodo
{
	public class RequestHandler : RequestHandlerBase<GetTodoRequest, GetTodoResponse>
	{
		public RequestHandler(IQueryRepositoryFactory queryRepositoryFactory)
			: base(queryRepositoryFactory)
		{
		}

		public override async Task<GetTodoResponse> Handle(GetTodoRequest request, CancellationToken cancellationToken)
		{
			var queryRepository = QueryRepositoryFactory.QueryEfRepository<Domain.Todo>();
			var result = await queryRepository.GetByIdAsync(request.Id);
			return new GetTodoResponse
			{
				Result = result.ToDto()
			};
		}
	}
}
