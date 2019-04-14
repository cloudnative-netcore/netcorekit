using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetCoreKit.Domain;

namespace NetCoreKit.Infrastructure.EfCore.Repository
{
    public class UnitOfWork : IUnitOfWorkAsync
    {
        private readonly DbContext _context;
        private ConcurrentDictionary<Type, object> _repositories;

        public UnitOfWork(DbContext context)
        {
            _context = context;
        }

        public virtual IRepositoryWithTypeAsync<TEntity, TId> RepositoryAsync<TEntity, TId>() where TEntity : class, IAggregateRootWithType<TId>
        {
            if (_repositories == null)
                _repositories = new ConcurrentDictionary<Type, object>();
            var type = typeof(TEntity);
            if (!_repositories.ContainsKey(type))
                _repositories[type] = new RepositoryWithTypeAsync<DbContext, TEntity, TId>(_context);

            return (IRepositoryWithTypeAsync<TEntity, TId>)_repositories[type];
        }

        public virtual IRepositoryAsync<TEntity> RepositoryAsync<TEntity>() where TEntity : class, IAggregateRoot
        {
            if (_repositories == null) _repositories = new ConcurrentDictionary<Type, object>();
            var type = typeof(TEntity);
            if (!_repositories.ContainsKey(type)) _repositories[type] = new RepositoryAsync<DbContext, TEntity>(_context);

            return (IRepositoryAsync<TEntity>)_repositories[type];
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

    public class RepositoryAsync<TDbContext, TEntity> : RepositoryWithTypeAsync<TDbContext, TEntity, Guid>
        where TDbContext : DbContext
        where TEntity : class, IAggregateRootWithType<Guid>
    {
        public RepositoryAsync(TDbContext dbContext) : base(dbContext)
        {
        }
    }

    public class RepositoryWithTypeAsync<TDbContext, TEntity, TId> : IRepositoryWithTypeAsync<TEntity, TId>
        where TDbContext : DbContext
        where TEntity : class, IAggregateRootWithType<TId>
    {
        private readonly TDbContext _dbContext;
        private readonly DbSet<TEntity> _dbSet;

        public RepositoryWithTypeAsync(TDbContext dbContext)
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
