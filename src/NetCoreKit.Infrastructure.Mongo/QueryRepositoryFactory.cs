using System;
using Microsoft.Extensions.DependencyInjection;
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

        public IQueryRepository<TEntity> QueryRepository<TEntity>() where TEntity : class, IAggregateRoot
        {
            return _serviceProvider.GetService<IQueryRepository<TEntity>>();
        }

        public IQueryRepositoryWithId<TEntity, TId> QueryRepository<TEntity, TId>() where TEntity : class, IAggregateRootWithId<TId>
        {
            return _serviceProvider.GetService<IQueryRepositoryWithId<TEntity, TId>>();
        }
    }
}
