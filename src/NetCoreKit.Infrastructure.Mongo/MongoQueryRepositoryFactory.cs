using System;
using NetCoreKit.Domain;

namespace NetCoreKit.Infrastructure.Mongo
{
    public class MongoQueryRepositoryFactory : IQueryRepositoryFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public MongoQueryRepositoryFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IQueryRepository<TEntity> QueryRepository<TEntity>() where TEntity : IAggregateRoot
        {
            return _serviceProvider.GetService(typeof(IQueryRepository<TEntity>)) as IQueryRepository<TEntity>;
        }
    }
}
