using System.ComponentModel.DataAnnotations;
using MediatR;
using NetCoreKit.Samples.TodoAPI.Dtos;

namespace NetCoreKit.Samples.TodoAPI.v1.UseCases.AddTodo
{
  public class AddTodoRequest : IRequest<AddTodoResponse>
	{
	  public AddTodoRequest()
	  {
	    Completed = false;
	    Order = 1;
	    Title = "sample todo";
	  }

		public int? Order { get; set; }
		[Required] public string Title { get; set; }
		public bool? Completed { get; set; }
	}

	public class AddTodoResponse
	{
		public TodoDto Result { get; set; }
	}
}
