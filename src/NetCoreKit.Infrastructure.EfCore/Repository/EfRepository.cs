using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

    public EfRepositoryAsync(TDbContext dbContext)
    {
      _dbContext = dbContext;
      _dbContext.ChangeTracker.AutoDetectChangesEnabled = false;
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
      await _dbContext.Set<TEntity>().AddAsync(entity);
      return entity;
    }

    public async Task<int> CountAsync()
    {
      return await _dbContext.Set<TEntity>().CountAsync();
    }

    public async Task<TEntity> DeleteAsync(TEntity entity)
    {
      _dbContext.Entry(entity).State = EntityState.Deleted;
      return await Task.FromResult(entity);
    }

    public bool Exist(Expression<Func<TEntity, bool>> predicate)
    {
      var exist = _dbContext.Set<TEntity>().Where(predicate);
      return exist.Any() ? true : false;
    }

    public async Task<ICollection<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> match)
    {
      return await _dbContext.Set<TEntity>().Where(match).ToListAsync();
    }

    public async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> match)
    {
      return await _dbContext.Set<TEntity>().SingleOrDefaultAsync(match);
    }

    public async Task<ICollection<TEntity>> GetAllAsync()
    {
      return await _dbContext.Set<TEntity>().ToListAsync();
    }

    public async Task<TEntity> GetByIdAsync(int id)
    {
      return await _dbContext.Set<TEntity>().FindAsync(id);
    }

    public async Task<TEntity> GetByUniqueIdAsync(string id)
    {
      return await _dbContext.Set<TEntity>().FindAsync(id);
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
      _dbContext.Entry(entity).State = EntityState.Modified;
      return await Task.FromResult(entity);
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
      _dbContext.ChangeTracker.AutoDetectChangesEnabled = false;
    }

    public IQueryable<TEntity> Queryable() => _dbContext.Set<TEntity>();
  }
}
