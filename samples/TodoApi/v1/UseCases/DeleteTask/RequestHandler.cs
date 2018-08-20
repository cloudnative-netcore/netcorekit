using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetCoreKit.Domain;
using NetCoreKit.Infrastructure.AspNetCore.CleanArch;
using NetCoreKit.Infrastructure.EfCore.Extensions;
using NetCoreKit.Samples.TodoAPI.Domain;

namespace NetCoreKit.Samples.TodoAPI.v1.UseCases.DeleteTask
{
  public class RequestHandler : TxRequestHandlerBase<DeleteTaskRequest, DeleteTaskResponse>
  {
    public RequestHandler(IUnitOfWorkAsync uow, IQueryRepositoryFactory queryRepositoryFactory)
      : base(uow, queryRepositoryFactory)
    {
    }

    public override async Task<DeleteTaskResponse> Handle(DeleteTaskRequest request,
      CancellationToken cancellationToken)
    {
      var projectRepository = UnitOfWork.Repository<Project>();
      var taskRepository = UnitOfWork.Repository<Domain.Task>();
      var queryRepository = QueryRepositoryFactory.QueryEfRepository<Project>();

      var project = await queryRepository.GetByIdAsync(request.ProjectId, q => q.Include(x => x.Tasks));
      if (project == null) throw new Exception($"Couldn't find the project#{request.ProjectId}.");

      foreach (var projectTask in project.Tasks)
      {
        if(request.TaskId == projectTask.Id)
          await taskRepository.DeleteAsync(projectTask);
      }

      project.RemoveTask(request.TaskId);
      var result = await projectRepository.UpdateAsync(project);
      await UnitOfWork.SaveChangesAsync(cancellationToken);

      return new DeleteTaskResponse {Result = result.Id};
    }
  }
}
