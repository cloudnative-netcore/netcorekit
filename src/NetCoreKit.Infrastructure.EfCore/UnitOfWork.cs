using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetCoreKit.Domain;

namespace NetCoreKit.Infrastructure.EfCore
{
    public class UnitOfWork : IUnitOfWorkAsync
    {
        private readonly DbContext _context;
        private ConcurrentDictionary<Type, object> _repositories;

        public UnitOfWork(DbContext context)
        {
            _context = context;
        }

        public virtual IRepositoryWithIdAsync<TEntity, TId> RepositoryAsync<TEntity, TId>() where TEntity : class, IAggregateRootWithId<TId>
        {
            if (_repositories == null) _repositories = new ConcurrentDictionary<Type, object>();

            if (!_repositories.ContainsKey(typeof(TEntity)))
                _repositories[typeof(TEntity)] = new RepositoryWithIdAsync<DbContext, TEntity, TId>(_context);

            return (IRepositoryWithIdAsync<TEntity, TId>)_repositories[typeof(TEntity)];
        }

        public virtual IRepositoryAsync<TEntity> RepositoryAsync<TEntity>()
            where TEntity : class, IAggregateRoot
        {
            if (_repositories == null) _repositories = new ConcurrentDictionary<Type, object>();

            if (!_repositories.ContainsKey(typeof(TEntity)))
                _repositories[typeof(TEntity)] = new RepositoryAsync<TEntity>(_context);

            return (IRepositoryAsync<TEntity>)_repositories[typeof(TEntity)];
        }

        public virtual int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public virtual Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }

    public class RepositoryAsync<TEntity> : RepositoryAsync<DbContext, TEntity>
        where TEntity : class, IAggregateRoot
    {
        public RepositoryAsync(DbContext dbContext) : base(dbContext)
        {
        }
    }

    public class RepositoryAsync<TDbContext, TEntity> : RepositoryWithIdAsync<TDbContext, TEntity, Guid>, IRepositoryAsync<TEntity>
        where TDbContext : DbContext
        where TEntity : class, IAggregateRoot
    {
        public RepositoryAsync(TDbContext dbContext) : base(dbContext)
        {
        }
    }

    public class RepositoryWithIdAsync<TDbContext, TEntity, TId> : IRepositoryWithIdAsync<TEntity, TId>
        where TDbContext : DbContext
        where TEntity : class, IAggregateRootWithId<TId>
    {
        private readonly TDbContext _dbContext;
        private readonly DbSet<TEntity> _dbSet;

        public RepositoryWithIdAsync(TDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<TEntity>();
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public async Task<TEntity> DeleteAsync(TEntity entity)
        {
            var entry = _dbSet.Remove(entity);
            return await Task.FromResult(entry.Entity);
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            var entry = _dbContext.Entry(entity);
            entry.State = EntityState.Modified;
            return await Task.FromResult(entry.Entity);
        }
    }
}
