using System;
using System.ComponentModel.DataAnnotations;
using MediatR;
using NetCoreKit.Infrastructure.AspNetCore.OpenApi;
using NetCoreKit.Samples.TodoAPI.Dtos;

namespace NetCoreKit.Samples.TodoAPI.v1.UseCases.UpdateTask
{
    public class UpdateTaskRequest : IRequest<UpdateTaskResponse>
    {
        [SwaggerExclude] public Guid ProjectId { get; set; }

        internal Guid TaskId { get; set; }
        public int? Order { get; set; } = 1;
        [Required] public string Title { get; set; }
        public bool? Completed { get; set; } = false;
    }

    public class UpdateTaskResponse
    {
        public ProjectDto Result { get; set; }
    }
}
