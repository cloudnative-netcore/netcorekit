using System;
using MediatR;
using NetCoreKit.Infrastructure.AspNetCore.OpenApi;
using NetCoreKit.Samples.TodoAPI.Dtos;

namespace NetCoreKit.Samples.TodoAPI.v1.UseCases.GetTasks
{
	public class GetTasksRequest : IRequest<GetTasksResponse>
	{
	  [SwaggerExclude]
    public Guid ProjectId { get; set; }
	}

	public class GetTasksResponse
	{
		public ProjectDto Result { get; set; }
	}
}
