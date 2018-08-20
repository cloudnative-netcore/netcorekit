using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetCoreKit.Domain;
using NetCoreKit.Infrastructure.AspNetCore.CleanArch;
using NetCoreKit.Infrastructure.EfCore.Extensions;
using NetCoreKit.Samples.TodoAPI.Domain;
using NetCoreKit.Samples.TodoAPI.Extensions;
using Task = NetCoreKit.Samples.TodoAPI.Domain.Task;

namespace NetCoreKit.Samples.TodoAPI.v1.UseCases.AddTask
{
  public class RequestHandler : TxRequestHandlerBase<AddTaskRequest, AddTaskResponse>
  {
    private readonly IUserGateway _userGateway;

    public RequestHandler(
      IUnitOfWorkAsync uow,
      IQueryRepositoryFactory queryRepositoryFactory,
      IUserGateway userGateway)
      : base(uow, queryRepositoryFactory)
    {
      _userGateway = userGateway;
    }

    public override async Task<AddTaskResponse> Handle(AddTaskRequest request, CancellationToken cancellationToken)
    {
      var projectRepository = UnitOfWork.Repository<Project>();
      var taskRepository = UnitOfWork.Repository<Task>();
      var projectQueryRepository = QueryRepositoryFactory.QueryEfRepository<Project>();

      var project = await projectQueryRepository.GetByIdAsync(request.ProjectId, q => q.Include(x => x.Tasks));
      if (project == null)
        throw new Exception($"Couldn't found the project#{request.ProjectId}.");

      var author = await _userGateway.GetAuthorAsync();
      if (author == null) throw new Exception("Couldn't found the default author.");

      var task = Task.Load(request.Title);
      task = task.SetAuthor(author.Id, author.GetFullName());

      task = await taskRepository.AddAsync(task);
      project.AddTask(task);
      project = await projectRepository.UpdateAsync(project);
      await UnitOfWork.SaveChangesAsync(cancellationToken);

      return new AddTaskResponse {Result = project.ToDto()};
    }
  }
}
