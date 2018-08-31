using System.Linq;
using System.Threading.Tasks;

namespace NetCoreKit.Domain
{
  public interface IRepositoryFactory
  {
    IRepositoryAsync<TEntity> Repository<TEntity>() where TEntity : class, IAggregateRoot;
  }

  public interface IQueryRepositoryFactory
  {
    IQueryRepository<TEntity> QueryRepository<TEntity>() where TEntity : IAggregateRoot;
  }

  public interface IRepositoryAsync<TEntity> where TEntity : IAggregateRoot
  {
    Task<TEntity> AddAsync(TEntity entity);
    Task<TEntity> UpdateAsync(TEntity entity);
    Task<TEntity> DeleteAsync(TEntity entity);
  }

  public interface IQueryRepository<out TEntity> where TEntity : IAggregateRoot
  {
    IQueryable<TEntity> Queryable();
  }
}
