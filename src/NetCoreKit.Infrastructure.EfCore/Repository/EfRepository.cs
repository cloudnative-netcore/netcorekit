using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetCoreKit.Domain;

namespace NetCoreKit.Infrastructure.EfCore.Repository
{
  public class EfQueryRepositoryFactory : IQueryRepositoryFactory
  {
    private readonly IServiceProvider _serviceProvider;

    public EfQueryRepositoryFactory(IServiceProvider serviceProvider)
    {
      _serviceProvider = serviceProvider;
    }

    public IQueryRepository<TEntity> QueryRepository<TEntity>() where TEntity : IEntity
    {
      return _serviceProvider.GetService(typeof(IEfQueryRepository<TEntity>)) as IEfQueryRepository<TEntity>;
    }
  }

  public class EfRepositoryAsync<TEntity>
    : EfRepositoryAsync<DbContext, TEntity>, IEfRepositoryAsync<TEntity>
    where TEntity : class, IEntity
  {
    public EfRepositoryAsync(DbContext dbContext) : base(dbContext)
    {
    }
  }

  public class EfQueryRepository<TEntity>
    : EfQueryRepository<DbContext, TEntity>, IEfQueryRepository<TEntity>
    where TEntity : class, IEntity
  {
    public EfQueryRepository(DbContext dbContext) : base(dbContext)
    {
    }
  }

  public class EfRepositoryAsync<TDbContext, TEntity> : IEfRepositoryAsync<TDbContext, TEntity>
    where TDbContext : DbContext
    where TEntity : class, IEntity
  {
    private readonly TDbContext _dbContext;
    private readonly DbSet<TEntity> _dbSet;

    public EfRepositoryAsync(TDbContext dbContext)
    {
      _dbContext = dbContext;
      _dbSet = _dbContext.Set<TEntity>();
      //_dbContext.ChangeTracker.AutoDetectChangesEnabled = false;
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
      await _dbSet.AddAsync(entity);
      return entity;
    }

    public async Task<TEntity> DeleteAsync(TEntity entity)
    {
      // _dbContext.Entry(entity).State = EntityState.Deleted;
      var entry = _dbSet.Remove(entity);
      return await Task.FromResult(entry.Entity);
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
      // _dbContext.Entry(entity).State = EntityState.Modified;
      var entry = _dbSet.Update(entity);
      return await Task.FromResult(entry.Entity);
    }
  }

  public class EfQueryRepository<TDbContext, TEntity> : IEfQueryRepository<TDbContext, TEntity>
    where TDbContext : DbContext
    where TEntity : class, IEntity
  {
    private readonly TDbContext _dbContext;

    public EfQueryRepository(TDbContext dbContext)
    {
      _dbContext = dbContext;
      //_dbContext.ChangeTracker.AutoDetectChangesEnabled = false;
    }

    public IQueryable<TEntity> Queryable() => _dbContext.Set<TEntity>();
  }
}
