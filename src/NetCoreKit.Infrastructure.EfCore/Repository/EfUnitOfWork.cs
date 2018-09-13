using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NetCoreKit.Domain;

namespace NetCoreKit.Infrastructure.EfCore.Repository
{
  public class EfUnitOfWork : IUnitOfWorkAsync
  {
    private readonly DbContext _context;
    protected IDbContextTransaction Transaction;
    private ConcurrentDictionary<Type, object> _repositories;

    public EfUnitOfWork(DbContext context)
    {
      _context = context;
    }

    public virtual IRepositoryAsync<TEntity> Repository<TEntity>() where TEntity : class, IAggregateRoot
    {
      if (_repositories == null)
      {
        _repositories = new ConcurrentDictionary<Type, object>();
      }
      var type = typeof(TEntity);
      if (!_repositories.ContainsKey(type))
      {
        _repositories[type] = new EfRepositoryAsync<TEntity>(_context);
      }

      return (IRepositoryAsync<TEntity>)_repositories[type];
    }

    public int? CommandTimeout
    {
      get => _context.Database.GetCommandTimeout();
      set => _context.Database.SetCommandTimeout(value);
    }

    /*public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
    {
      return _context.Database.BeginTransactionAsync(cancellationToken);
    }*/

    public virtual int SaveChanges() => _context.SaveChanges();

    public virtual Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
      return _context.SaveChangesAsync(cancellationToken);
    }

    public virtual int ExecuteSqlCommand(string sql, params object[] parameters)
    {
      return _context.Database.ExecuteSqlCommand(sql, parameters);
    }

    public virtual async Task<int> ExecuteSqlCommandAsync(string sql, params object[] parameters)
    {
      return await _context.Database.ExecuteSqlCommandAsync(sql, parameters);
    }

    public virtual async Task<int> ExecuteSqlCommandAsync(string sql, CancellationToken cancellationToken, params object[] parameters)
    {
      return await _context.Database.ExecuteSqlCommandAsync(sql, cancellationToken, parameters);
    }

    public void Dispose()
    {
      // Transaction?.Dispose();
      _context?.Dispose();
    }
  }
}
