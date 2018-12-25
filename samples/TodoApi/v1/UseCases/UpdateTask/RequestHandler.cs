using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetCoreKit.Domain;
using NetCoreKit.Infrastructure.AspNetCore.CleanArch;
using NetCoreKit.Infrastructure.EfCore.Extensions;
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
            var commandRepository = CommandFactory.RepositoryAsync<Domain.Project>();
            var queryRepository = QueryFactory.QueryEfRepository<Domain.Project>();

            var project = await queryRepository.GetByIdAsync(request.ProjectId, q => q.Include(x => x.Tasks), false);
            if (project == null) throw new Exception($"Couldn't find project#{request.ProjectId}.");

            project.UpdateTask(request.TaskId, request.Title, request.Order ?? 1, request.Completed ?? false);
            var updated = await commandRepository.UpdateAsync(project);

            return new UpdateTaskResponse {Result = updated.ToDto()};
        }
    }
}
