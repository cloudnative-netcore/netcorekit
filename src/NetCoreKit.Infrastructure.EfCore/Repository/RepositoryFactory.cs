using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NetCoreKit.Domain;

namespace NetCoreKit.Infrastructure.EfCore.Repository
{
    public class QueryRepositoryFactory : IQueryRepositoryFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public QueryRepositoryFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IQueryRepository<TEntity> QueryRepository<TEntity>() where TEntity : IAggregateRoot
        {
            return _serviceProvider.GetService(typeof(IQueryRepository<TEntity>)) as IQueryRepository<TEntity>;
        }

        public IQueryRepositoryWithType<TEntity, TId> QueryRepository<TEntity, TId>() where TEntity : IAggregateRootWithType<TId>
        {
            return _serviceProvider.GetService(typeof(IQueryRepositoryWithType<TEntity, TId>)) as IQueryRepositoryWithType<TEntity, TId>;
        }
    }

    public class QueryRepository<TDbContext, TEntity> : QueryRepositoryWithType<TDbContext, TEntity, Guid>
        where TDbContext : DbContext
        where TEntity : class, IAggregateRootWithType<Guid>
    {
        public QueryRepository(TDbContext dbContext) : base(dbContext)
        {
        }
    }

    public class QueryRepositoryWithType<TDbContext, TEntity, TId> : IQueryRepositoryWithType<TEntity, TId>
        where TDbContext : DbContext
        where TEntity : class, IAggregateRootWithType<TId>
    {
        private readonly TDbContext _dbContext;

        public QueryRepositoryWithType(TDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<TEntity> Queryable()
        {
            return _dbContext.Set<TEntity>();
        }
    }

    /*public class EfRepositoryAsync<TEntity> : EfRepositoryAsync<DbContext, TEntity>, IEfRepositoryAsync<TEntity>
        where TEntity : class, IAggregateRoot
    {
        public EfRepositoryAsync(DbContext dbContext) : base(dbContext)
        {
        }
    }*/

    /*public class EfQueryRepository<TEntity> : EfQueryRepository<DbContext, TEntity>, IEfQueryRepository<TEntity>
        where TEntity : class, IAggregateRoot
    {
        public EfQueryRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }*/
}
