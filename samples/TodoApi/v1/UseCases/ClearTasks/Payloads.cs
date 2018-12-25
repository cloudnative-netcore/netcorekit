using System;
using MediatR;
using NetCoreKit.Infrastructure.AspNetCore.OpenApi;

namespace NetCoreKit.Samples.TodoAPI.v1.UseCases.ClearTasks
{
    public class ClearTasksRequest : IRequest<ClearTasksResponse>
    {
        [SwaggerExclude] public Guid ProjectId { get; set; }
    }

    public class ClearTasksResponse
    {
        public bool Result { get; set; } = true;
    }
}
