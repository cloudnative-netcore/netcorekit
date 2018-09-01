using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using NetCoreKit.Domain;
using NetCoreKit.Samples.TodoAPI.Domain;
using NetCoreKit.Samples.TodoAPI.Extensions;
using Task = System.Threading.Tasks.Task;

namespace NetCoreKit.Samples.TodoAPI.v1.UseCases.CreateProject
{
  public class RequestHandler : IRequestHandler<CreateProjectRequest, CreateProjectResponse>
  {
    private readonly IUnitOfWorkAsync _uow;

    public RequestHandler(IUnitOfWorkAsync uow)
    {
      _uow = uow;
    }

    public async Task<CreateProjectResponse> Handle(CreateProjectRequest request,
      CancellationToken cancellationToken)
    {
      var projectRepository = _uow.Repository<Project>();

      var result = await projectRepository.AddAsync(Project.Load(request.Name));
      await _uow.SaveChangesAsync(cancellationToken);

      return new CreateProjectResponse {Result = result.ToDto()};
    }
  }

  /// <summary>
  /// Due to In-memory mode
  /// </summary>
  public class ProjectCreatedProfile : Profile
  {
    public ProjectCreatedProfile()
    {
      CreateMap<ProjectCreated, ProjectCreated>();
    }
  }

  public class ProjectCreatedSubscriber : INotificationHandler<ProjectCreated>
  {
    public Task Handle(ProjectCreated @event, CancellationToken cancellationToken)
    {
      // do something with @event
      //...

      return Task.FromResult(@event);
    }
  }
}
