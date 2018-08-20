using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetCoreKit.Domain;
using NetCoreKit.Infrastructure.AspNetCore.CleanArch;
using NetCoreKit.Infrastructure.EfCore.Extensions;
using NetCoreKit.Samples.TodoAPI.Domain;
using NetCoreKit.Samples.TodoAPI.Extensions;

namespace NetCoreKit.Samples.TodoAPI.v1.UseCases.UpdateTask
{
  public class RequestHandler : TxRequestHandlerBase<UpdateTaskRequest, UpdateTaskResponse>
  {
    public RequestHandler(IUnitOfWorkAsync uow, IQueryRepositoryFactory queryRepositoryFactory)
      : base(uow, queryRepositoryFactory)
    {
    }

    public override async Task<UpdateTaskResponse> Handle(UpdateTaskRequest request,
      CancellationToken cancellationToken)
    {
      var commandRepository = UnitOfWork.Repository<Project>();
      var queryRepository = QueryRepositoryFactory.QueryEfRepository<Project>();
      var taskQueryRepository = QueryRepositoryFactory.QueryEfRepository<Domain.Task>();

      var project = await queryRepository.GetByIdAsync(request.TaskId, q => q.Include(x => x.Tasks));
      if (project == null) throw new Exception($"Could not find project#{request.TaskId}.");

      var task = await taskQueryRepository.GetByIdAsync(request.TaskId);
      task.ChangeTitle(request.Title)
        .ChangeOrder(request.Order ?? 1);

      if (request.Completed.HasValue && request.Completed.Value)
        task.ChangeToCompleted();

      project.UpdateTask(task);
      var updated = await commandRepository.UpdateAsync(project);
      await UnitOfWork.SaveChangesAsync(cancellationToken);

      return new UpdateTaskResponse {Result = updated.ToDto()};
    }
  }
}
