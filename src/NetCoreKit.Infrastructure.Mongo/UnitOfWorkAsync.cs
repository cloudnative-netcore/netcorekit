using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using NetCoreKit.Domain;

namespace NetCoreKit.Infrastructure.Mongo
{
    public class UnitOfWorkAsync : IUnitOfWorkAsync
    {
        private readonly MongoContext _dbContext;
        private readonly IDomainEventDispatcher _domainEventDispatcher;
        private ConcurrentDictionary<Type, object> _repositories;

        public UnitOfWorkAsync(MongoContext dbContext, IDomainEventDispatcher domainEventDispatcher)
        {
            _dbContext = dbContext;
            _domainEventDispatcher = domainEventDispatcher;
        }

        public virtual IRepositoryAsync<TEntity> RepositoryAsync<TEntity>()
            where TEntity : class, IAggregateRoot
        {
            if (_repositories == null) _repositories = new ConcurrentDictionary<Type, object>();
            var type = typeof(TEntity);
            if (!_repositories.ContainsKey(type)) _repositories[type] = new Repository<TEntity>(_dbContext, _domainEventDispatcher);

            return (IRepositoryAsync<TEntity>)_repositories[type];
        }

        public virtual IRepositoryWithTypeAsync<TEntity, TId> RepositoryAsync<TEntity, TId>()
            where TEntity : class, IAggregateRootWithType<TId>
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            if (_dbContext != null) GC.SuppressFinalize(_dbContext);
        }

        public int SaveChanges()
        {
            // we do nothing
            return 1;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            // we do nothing
            return Task.FromResult(1);
        }
    }
}
