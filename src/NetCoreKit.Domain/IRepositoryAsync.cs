using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NetCoreKit.Domain
{
  public interface IRepositoryFactory
  {
    IRepositoryAsync<TEntity> Repository<TEntity>() where TEntity : IEntity;
  }

  public interface IQueryRepositoryFactory
  {
    IQueryRepository<TEntity> QueryRepository<TEntity>() where TEntity : IEntity;
  }

  public interface IRepositoryAsync<TEntity> where TEntity : IEntity
  {
    Task<ICollection<TEntity>> GetAllAsync();
    Task<TEntity> GetByIdAsync(int id);
    Task<TEntity> GetByUniqueIdAsync(string id);
    Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> match);
    Task<ICollection<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> match);
    Task<int> CountAsync();
    bool Exist(Expression<Func<TEntity, bool>> predicate);
    Task<TEntity> AddAsync(TEntity entity);
    Task<TEntity> UpdateAsync(TEntity entity);
    Task<TEntity> DeleteAsync(TEntity entity);
  }

  public interface IQueryRepository<TEntity> where TEntity : IEntity
  {
    IQueryable<TEntity> Queryable();
  }
}
