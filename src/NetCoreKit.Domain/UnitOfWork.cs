using System;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreKit.Domain
{
    public interface IUnitOfWorkAsync : IRepositoryFactoryAsync, IDisposable
    {
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }

    public interface IRepositoryFactoryAsync
    {
        IRepositoryWithTypeAsync<TEntity, TId> RepositoryAsync<TEntity, TId>() where TEntity : class, IAggregateRootWithType<TId>;
        IRepositoryAsync<TEntity> RepositoryAsync<TEntity>() where TEntity : class, IAggregateRoot;
    }

    public interface IRepositoryAsync<TEntity> : IRepositoryWithTypeAsync<TEntity, Guid> where TEntity : IAggregateRoot
    {
    }

    public interface IRepositoryWithTypeAsync<TEntity, TId> where TEntity : IAggregateRootWithType<TId>
    {
        Task<TEntity> AddAsync(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity);
        Task<TEntity> DeleteAsync(TEntity entity);
    }
}
