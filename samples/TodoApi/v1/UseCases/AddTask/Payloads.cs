using System;
using System.ComponentModel.DataAnnotations;
using MediatR;
using NetCoreKit.Infrastructure.AspNetCore.OpenApi;
using NetCoreKit.Samples.TodoAPI.Dtos;

namespace NetCoreKit.Samples.TodoAPI.v1.UseCases.AddTask
{
    public class AddTaskRequest : IRequest<AddTaskResponse>
    {
        public AddTaskRequest()
        {
            Completed = false;
            Order = 1;
            Title = "sample todo";
            AuthorId = new Guid("E8F0B717-E325-466B-A87C-1AF1AA951599"); // we have it in db
        }

        [SwaggerExclude] public Guid ProjectId { get; set; }

        public int? Order { get; set; }
        [Required] public string Title { get; set; }
        public bool? Completed { get; set; }
        public Guid AuthorId { get; }
    }

    public class AddTaskResponse
    {
        public ProjectDto Result { get; set; }
    }
}
