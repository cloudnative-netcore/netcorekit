using System;
using MediatR;
using NetCoreKit.Samples.TodoAPI.Dtos;

namespace NetCoreKit.Samples.TodoAPI.v1.UseCases.GetTodo
{
	public class GetTodoRequest : IRequest<GetTodoResponse>
	{
		public Guid Id { get; set; }
	}

	public class GetTodoResponse
	{
		public TodoDto Result { get; set; }
	}
}