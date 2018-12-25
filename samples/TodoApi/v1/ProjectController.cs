using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetCoreKit.Infrastructure.AspNetCore.CleanArch;
using NetCoreKit.Samples.TodoAPI.v1.UseCases.AddTask;
using NetCoreKit.Samples.TodoAPI.v1.UseCases.ClearTasks;
using NetCoreKit.Samples.TodoAPI.v1.UseCases.CreateProject;
using NetCoreKit.Samples.TodoAPI.v1.UseCases.DeleteTask;
using NetCoreKit.Samples.TodoAPI.v1.UseCases.GetTasks;
using NetCoreKit.Samples.TodoAPI.v1.UseCases.UpdateTask;

namespace NetCoreKit.Samples.TodoAPI.v1
{
    [ApiVersion("1.0")]
    [Route("api/projects")]
    public class ProjectController : Controller
    {
        private readonly ILogger<ProjectController> _logger;

        public ProjectController(ILoggerFactory factory)
        {
            _logger = factory.CreateLogger<ProjectController>();
        }

        [HttpPost]
        public async Task<IActionResult> PostProject([FromServices] IMediator eventor, CreateProjectRequest request,
            CancellationToken cancellationToken)
        {
            return await eventor.SendStream<CreateProjectRequest, CreateProjectResponse>(
                request,
                x => x.Result,
                cancellationToken);
        }

        [HttpGet("{projectId:guid}/tasks")]
        public async Task<IActionResult> GetTasks([FromServices] IMediator eventor, Guid projectId,
            CancellationToken cancellationToken)
        {
            return await eventor.SendStream<GetTasksRequest, GetTasksResponse>(
                new GetTasksRequest {ProjectId = projectId},
                x => x.Result,
                cancellationToken);
        }

        [HttpPost("{projectId:guid}/tasks")]
        public async Task<IActionResult> PostTask([FromServices] IMediator eventor, Guid projectId,
            AddTaskRequest request,
            CancellationToken cancellationToken)
        {
            request.ProjectId = projectId;
            return await eventor.SendStream<AddTaskRequest, AddTaskResponse>(
                request,
                x => x.Result,
                cancellationToken);
        }

        [HttpPut("{projectId:guid}/tasks/{taskId:guid}")]
        public async Task<IActionResult> PutTask([FromServices] IMediator eventor, Guid projectId, Guid taskId,
            UpdateTaskRequest request, CancellationToken cancellationToken)
        {
            request.ProjectId = projectId;
            request.TaskId = taskId;
            return await eventor.SendStream<UpdateTaskRequest, UpdateTaskResponse>(
                request,
                x => x.Result,
                cancellationToken);
        }

        [HttpDelete("{projectId:guid}/tasks/{taskId:guid}")]
        public async Task<IActionResult> DeleteTask([FromServices] IMediator eventor, Guid projectId, Guid taskId,
            CancellationToken cancellationToken)
        {
            return await eventor.SendStream<DeleteTaskRequest, DeleteTaskResponse>(
                new DeleteTaskRequest
                {
                    ProjectId = projectId,
                    TaskId = taskId
                },
                x => x.Result,
                cancellationToken);
        }

        [HttpDelete("{projectId:guid}/tasks")]
        public async Task<IActionResult> ClearTasks([FromServices] IMediator eventor, Guid projectId,
            CancellationToken cancellationToken)
        {
            return await eventor.SendStream<ClearTasksRequest, ClearTasksResponse>(
                new ClearTasksRequest {ProjectId = projectId},
                x => x.Result);
        }
    }
}
