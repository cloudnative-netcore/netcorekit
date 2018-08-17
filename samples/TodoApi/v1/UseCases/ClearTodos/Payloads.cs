using MediatR;

namespace NetCoreKit.Samples.TodoAPI.v1.UseCases.ClearTodos
{
	public class ClearTodosRequest : IRequest<ClearTodosResponse>
	{
	}

	public class ClearTodosResponse
	{
	  public bool Result { get; set; } = true;
	}
}
