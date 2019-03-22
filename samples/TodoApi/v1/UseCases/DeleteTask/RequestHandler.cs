using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetCoreKit.Domain;
using NetCoreKit.Infrastructure.AspNetCore.CleanArch;
using NetCoreKit.Infrastructure.EfCore.Extensions;

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
            var projectRepository = CommandFactory.RepositoryAsync<Domain.Project>();
            var queryRepository = QueryFactory.QueryEfRepository<Domain.Project>();

            var project = await queryRepository.GetByIdAsync(request.ProjectId, q => q.Include(x => x.Tasks), false);
            if (project == null) throw new Exception($"Couldn't find the project#{request.ProjectId}.");

            project.RemoveTask(request.TaskId);
            var result = await projectRepository.UpdateAsync(project);

            return new DeleteTaskResponse {Result = result.Id};
        }
    }
}
