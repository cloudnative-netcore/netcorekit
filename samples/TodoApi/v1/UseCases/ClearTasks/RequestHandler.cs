using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetCoreKit.Domain;
using NetCoreKit.Infrastructure.AspNetCore.CleanArch;
using NetCoreKit.Infrastructure.EfCore.Extensions;
using NetCoreKit.Samples.TodoAPI.Domain;
using Task = NetCoreKit.Samples.TodoAPI.Domain.Task;

namespace NetCoreKit.Samples.TodoAPI.v1.UseCases.ClearTasks
{
  public class RequestHandler : TxRequestHandlerBase<ClearTasksRequest, ClearTasksResponse>
  {
    public RequestHandler(IUnitOfWorkAsync uow, IQueryRepositoryFactory queryRepositoryFactory)
      : base(uow, queryRepositoryFactory)
    {
    }

    public override async Task<ClearTasksResponse> Handle(ClearTasksRequest request,
      CancellationToken cancellationToken)
    {
      var projectRepository = UnitOfWork.Repository<Project>();
      var taskRepository = UnitOfWork.Repository<Task>();
      var queryRepository = QueryRepositoryFactory.QueryEfRepository<Project>();

      var project = await queryRepository.GetByIdAsync(request.ProjectId, q => q.Include(x => x.Tasks));
      if (project == null)
        throw new Exception($"Couldn't found the project#{request.ProjectId}.");

      foreach (var projectTask in project.Tasks)
        await taskRepository.DeleteAsync(projectTask);
      await UnitOfWork.SaveChangesAsync(cancellationToken);

      project.ClearTasks();
      await projectRepository.UpdateAsync(project);

      await UnitOfWork.SaveChangesAsync(cancellationToken);

      return new ClearTasksResponse();
    }
  }
}
