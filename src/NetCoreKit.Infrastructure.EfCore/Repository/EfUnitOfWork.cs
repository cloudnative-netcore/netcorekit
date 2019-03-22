using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetCoreKit.Domain;

namespace NetCoreKit.Infrastructure.EfCore.Repository
{
    public class EfUnitOfWork : IUnitOfWorkAsync
    {
        private readonly DbContext _context;
        private ConcurrentDictionary<Type, object> _repositories;

        public EfUnitOfWork(DbContext context)
        {
            _context = context;
        }

        public virtual IRepositoryAsync<TEntity> RepositoryAsync<TEntity>() where TEntity : class, IAggregateRoot
        {
            if (_repositories == null) _repositories = new ConcurrentDictionary<Type, object>();
            var type = typeof(TEntity);
            if (!_repositories.ContainsKey(type)) _repositories[type] = new EfRepositoryAsync<TEntity>(_context);

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
}
