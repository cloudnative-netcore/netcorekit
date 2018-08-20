using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetCoreKit.Domain;
using NetCoreKit.Infrastructure.AspNetCore.CleanArch;
using NetCoreKit.Infrastructure.EfCore.Extensions;
using NetCoreKit.Samples.TodoAPI.Domain;
using NetCoreKit.Samples.TodoAPI.Extensions;

namespace NetCoreKit.Samples.TodoAPI.v1.UseCases.GetTasks
{
	public class RequestHandler : RequestHandlerBase<GetTasksRequest, GetTasksResponse>
	{
		public RequestHandler(IQueryRepositoryFactory queryRepositoryFactory)
			: base(queryRepositoryFactory)
		{
		}

		public override async Task<GetTasksResponse> Handle(GetTasksRequest request, CancellationToken cancellationToken)
		{
			var queryRepository = QueryRepositoryFactory.QueryEfRepository<Project>();

		  var result = await queryRepository.GetByIdAsync(request.ProjectId, q => q.Include(x => x.Tasks));
		  if (result == null) throw new Exception($"Couldn't find project#{request.ProjectId}.");

			return new GetTasksResponse
			{
				Result = result.ToDto()
			};
		}
	}
}
