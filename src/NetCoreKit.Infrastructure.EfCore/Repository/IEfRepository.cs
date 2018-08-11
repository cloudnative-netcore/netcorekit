using Microsoft.EntityFrameworkCore;
using NetCoreKit.Domain;

namespace NetCoreKit.Infrastructure.EfCore.Repository
{
  public interface IEfRepositoryAsync<TEntity> : IEfRepositoryAsync<DbContext, TEntity>
        where TEntity : IEntity
  {
  }

  public interface IEfQueryRepository<TEntity> : IEfQueryRepository<DbContext, TEntity>
      where TEntity : IEntity
  {
  }

  public interface IEfRepositoryAsync<TDbContext, TEntity> : IRepositoryAsync<TEntity>
      where TDbContext : DbContext
      where TEntity : IEntity
  {
  }

  public interface IEfQueryRepository<TDbContext, TEntity> : IQueryRepository<TEntity>
      where TDbContext : DbContext
      where TEntity : IEntity
  {
  }
}
