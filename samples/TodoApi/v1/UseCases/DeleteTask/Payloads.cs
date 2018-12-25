using System;
using MediatR;
using NetCoreKit.Infrastructure.AspNetCore.OpenApi;

namespace NetCoreKit.Samples.TodoAPI.v1.UseCases.DeleteTask
{
    public class DeleteTaskRequest : IRequest<DeleteTaskResponse>
    {
        [SwaggerExclude] public Guid ProjectId { get; set; }

        public Guid TaskId { get; set; }
    }

    public class DeleteTaskResponse
    {
        public Guid Result { get; set; }
    }
}
