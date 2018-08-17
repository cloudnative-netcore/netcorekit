using System;
using System.ComponentModel.DataAnnotations;
using MediatR;
using NetCoreKit.Samples.TodoAPI.Dtos;

namespace NetCoreKit.Samples.TodoAPI.v1.UseCases.UpdateTodo
{
	public class UpdateTodoRequest : IRequest<UpdateTodoResponse>
	{
		public Guid Id { get; set; }
		public int? Order { get; set; } = 1;
		[Required] public string Title { get; set; }
		public bool? Completed { get; set; } = false;
	}

	public class UpdateTodoResponse
	{
		public TodoDto Result { get; set; }
	}
}
