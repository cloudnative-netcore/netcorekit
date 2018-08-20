using System.Threading;
using System.Threading.Tasks;
using NetCoreKit.Domain;
using NetCoreKit.Infrastructure.AspNetCore.CleanArch;
using NetCoreKit.Samples.TodoAPI.Domain;
using NetCoreKit.Samples.TodoAPI.Extensions;

namespace NetCoreKit.Samples.TodoAPI.v1.UseCases.CreateProject
{
  public class RequestHandler : TxRequestHandlerBase<CreateProjectRequest, CreateProjectResponse>
  {
    public RequestHandler(IUnitOfWorkAsync uow, IQueryRepositoryFactory queryRepositoryFactory)
      : base(uow, queryRepositoryFactory)
    {
    }

    public override async Task<CreateProjectResponse> Handle(CreateProjectRequest request,
      CancellationToken cancellationToken)
    {
      var projectRepository = UnitOfWork.Repository<Project>();

      var result = await projectRepository.AddAsync(Project.Load(request.Name));
      await UnitOfWork.SaveChangesAsync(cancellationToken);

      return new CreateProjectResponse {Result = result.ToDto()};
    }
  }
}
