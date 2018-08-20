using System.ComponentModel.DataAnnotations;
using MediatR;
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
}
