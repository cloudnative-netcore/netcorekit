using System.Collections.Generic;
using MediatR;
using NetCoreKit.Samples.TodoAPI.Dtos;

namespace NetCoreKit.Samples.TodoAPI.v1.UseCases.GetTodos
{
	public class GetTodosRequest : IRequest<GetTodosResponse>
	{
	}

	public class GetTodosResponse
	{
		public List<TodoDto> Result { get; set; }
	}
}