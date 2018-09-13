using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NetCoreKit.Domain;
using NetCoreKit.Samples.Contracts.Events;
using NetCoreKit.Samples.TodoAPI.Domain;
using NetCoreKit.Samples.TodoAPI.Extensions;

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

      return new CreateProjectResponse {Result = result.ToDto()};
    }
  }

  /*public class EventSubscriber : INotificationHandler<ProjectCreated>
  {
    public async System.Threading.Tasks.Task Handle(ProjectCreated @event, CancellationToken cancellationToken)
    {
      // do something with @event
      //...

      await System.Threading.Tasks.Task.FromResult(@event);
    }
  }*/
}
