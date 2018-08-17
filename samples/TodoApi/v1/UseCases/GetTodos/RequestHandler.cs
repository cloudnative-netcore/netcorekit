using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NetCoreKit.Domain;
using NetCoreKit.Infrastructure.AspNetCore.CleanArch;
using NetCoreKit.Infrastructure.EfCore.Extensions;
using NetCoreKit.Samples.TodoAPI.Extensions;

namespace NetCoreKit.Samples.TodoAPI.v1.UseCases.GetTodos
{
	public class RequestHandler : RequestHandlerBase<GetTodosRequest, GetTodosResponse>
	{
		public RequestHandler(IQueryRepositoryFactory queryRepositoryFactory)
			: base(queryRepositoryFactory)
		{
		}

		public override async Task<GetTodosResponse> Handle(GetTodosRequest request, CancellationToken cancellationToken)
		{
			var queryRepository = QueryRepositoryFactory.QueryEfRepository<Domain.Todo>();
			var result = await queryRepository.ListAsync();
			return new GetTodosResponse
			{
				Result = result.Select(x => x.ToDto()).ToList()
			};
		}
	}
}
