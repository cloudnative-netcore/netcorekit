using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NetCoreKit.Infrastructure.AspNetCore.CleanArch;
using NetCoreKit.Samples.TodoAPI.v1.UseCases.AddTodo;
using NetCoreKit.Samples.TodoAPI.v1.UseCases.ClearTodos;
using NetCoreKit.Samples.TodoAPI.v1.UseCases.DeleteTodo;
using NetCoreKit.Samples.TodoAPI.v1.UseCases.GetTodo;
using NetCoreKit.Samples.TodoAPI.v1.UseCases.GetTodos;
using NetCoreKit.Samples.TodoAPI.v1.UseCases.UpdateTodo;

namespace NetCoreKit.Samples.TodoAPI.v1
{
  [ApiVersion("1.0")]
  [Route("api/todos")]
  public class TodoController : Controller
  {
    [HttpGet]
    public async Task<IActionResult> Get([FromServices] IMediator eventor, CancellationToken cancellationToken)
    {
      return await eventor.SendStream<GetTodosRequest, GetTodosResponse>(
        new GetTodosRequest(),
        x => x.Result);
    }

    [HttpGet("{todoId:guid}")]
    public async Task<IActionResult> Get([FromServices] IMediator eventor, Guid todoId, CancellationToken cancellationToken)
    {
      return await eventor.SendStream<GetTodoRequest, GetTodoResponse>(
        new GetTodoRequest {Id = todoId},
        x => x.Result);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromServices] IMediator eventor, AddTodoRequest request, CancellationToken cancellationToken)
    {
      return await eventor.SendStream<AddTodoRequest, AddTodoResponse>(
        request,
        x => x.Result);
    }

    [HttpPut("{todoId:guid}")]
    public async Task<IActionResult> Put([FromServices] IMediator eventor, Guid todoId, UpdateTodoRequest request)
    {
      request.Id = todoId;
      return await eventor.SendStream<UpdateTodoRequest, UpdateTodoResponse>(
        request,
        x => x.Result);
    }

    [HttpDelete("{todoId:guid}")]
    public async Task<IActionResult> Delete([FromServices] IMediator eventor, Guid todoId, CancellationToken cancellationToken)
    {
      return await eventor.SendStream<DeleteTodoRequest, DeleteTodoResponse>(
        new DeleteTodoRequest {Id = todoId},
        x => x.Result);
    }

    [HttpDelete]
    public async Task<IActionResult> Clear([FromServices] IMediator eventor, CancellationToken cancellationToken)
    {
      return await eventor.SendStream<ClearTodosRequest, ClearTodosResponse>(
        new ClearTodosRequest(),
        x => x.Result);
    }
  }
}
