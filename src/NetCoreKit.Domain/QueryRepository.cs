using System;
using System.Linq;

namespace NetCoreKit.Domain
{
    public interface IQueryRepositoryFactory
    {
        IQueryRepositoryWithType<TEntity, TId> QueryRepository<TEntity, TId>() where TEntity : IAggregateRootWithType<TId>;
        IQueryRepository<TEntity> QueryRepository<TEntity>() where TEntity : IAggregateRoot;
    }

    public interface IQueryRepository<TEntity> : IQueryRepositoryWithType<TEntity, Guid>
        where TEntity : IAggregateRootWithType<Guid>
    {
    }

    public interface IQueryRepositoryWithType<TEntity, TId>
        where TEntity : IAggregateRootWithType<TId>
    {
        IQueryable<TEntity> Queryable();
    }
}
