using System;
using NetCoreKit.Domain;

namespace NetCoreKit.Infrastructure.Mongo
{
    public class QueryRepositoryFactory : IQueryRepositoryFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public QueryRepositoryFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IQueryRepository<TEntity> QueryRepository<TEntity>() where TEntity : IAggregateRoot
        {
            return _serviceProvider.GetService(typeof(IQueryRepository<TEntity>)) as IQueryRepository<TEntity>;
        }

        public IQueryRepositoryWithType<TEntity, TId> QueryRepository<TEntity, TId>() where TEntity : IAggregateRootWithType<TId>
        {
            throw new NotImplementedException();
        }
    }
}
