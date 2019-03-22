using System.ComponentModel.DataAnnotations;
using AutoMapper;
using MediatR;
using NetCoreKit.Infrastructure.Mappers;
using NetCoreKit.Samples.TodoAPI.Domain;
using NetCoreKit.Samples.TodoAPI.Dtos;

namespace NetCoreKit.Samples.TodoAPI.v1.UseCases.CreateProject
{
    public class CreateProjectRequest : IRequest<CreateProjectResponse>
    {
        public CreateProjectRequest()
        {
            Name = "sample project";
        }

        [Required] public string Name { get; set; }
    }

    public class CreateProjectResponse
    {
        public ProjectDto Result { get; set; }
    }

    public class CreateProjectProfile : Profile
    {
        public CreateProjectProfile()
        {
            this.MapMySelf<ProjectCreated>();
            this.MapMySelf<TaskCreated>();
        }
    }
}
