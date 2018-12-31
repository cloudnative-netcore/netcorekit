using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using NetCoreKit.Domain;

namespace NetCoreKit.Infrastructure.Mongo
{
    public class MongoUnitOfWorkAsync : IUnitOfWorkAsync
    {
        private readonly MongoContext _dbContext;
        private readonly IDomainEventBus _domainEventBus;
        private ConcurrentDictionary<Type, object> _repositories;

        public MongoUnitOfWorkAsync(MongoContext dbContext, IDomainEventBus domainEventBus)
        {
            _dbContext = dbContext;
            _domainEventBus = domainEventBus;
        }

        public virtual IRepositoryAsync<TEntity> RepositoryAsync<TEntity>() where TEntity : class, IAggregateRoot
        {
            if (_repositories == null) _repositories = new ConcurrentDictionary<Type, object>();
            var type = typeof(TEntity);
            if (!_repositories.ContainsKey(type)) _repositories[type] = new MongoRepositoryAsync<TEntity>(_dbContext, _domainEventBus);

            return (IRepositoryAsync<TEntity>)_repositories[type];
        }

        public void Dispose()
        {
            if (_dbContext != null) GC.SuppressFinalize(_dbContext);
        }

        public int SaveChanges()
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
