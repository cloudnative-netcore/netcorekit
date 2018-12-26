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
        private ConcurrentDictionary<Type, object> _repositories;

        public MongoUnitOfWorkAsync(MongoContext dbContext)
        {
            _dbContext = dbContext;
        }

        public virtual IRepositoryAsync<TEntity> RepositoryAsync<TEntity>() where TEntity : class, IAggregateRoot
        {
            if (_repositories == null) _repositories = new ConcurrentDictionary<Type, object>();
            var type = typeof(TEntity);
            if (!_repositories.ContainsKey(type)) _repositories[type] = new MongoRepositoryAsync<TEntity>(_dbContext);

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
