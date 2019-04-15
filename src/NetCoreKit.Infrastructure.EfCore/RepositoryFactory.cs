using System;
using System.Collections.Concurrent;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using NetCoreKit.Domain;

namespace NetCoreKit.Infrastructure.EfCore
{
    public class QueryRepositoryFactory : IQueryRepositoryFactory
    {
        private readonly DbContext _context;
        private ConcurrentDictionary<Type, object> _repositories;

        public QueryRepositoryFactory(DbContext context)
        {
            _context = context;
        }

        public IQueryRepository<TEntity> QueryRepository<TEntity>() where TEntity : class, IAggregateRoot
        {
            if (_repositories == null)
                _repositories = new ConcurrentDictionary<Type, object>();

            if (!_repositories.ContainsKey(typeof(TEntity)))
            {
                //var memoryCache = scope.ServiceProvider.GetService<IMemoryCache>();
                //var config = scope.ServiceProvider.GetService<IConfiguration>();
                //var cachedRepo = new CachedQueryRepository<TEntity>(new QueryRepository<TEntity>(_context), memoryCache, config);
                var cachedRepo = new QueryRepository<TEntity>(_context);
                _repositories[typeof(TEntity)] = cachedRepo;
            }

            return (IQueryRepository<TEntity>)_repositories[typeof(TEntity)];
        }

        public IQueryRepositoryWithId<TEntity, TId> QueryRepository<TEntity, TId>() where TEntity : class, IAggregateRootWithId<TId>
        {
            if (_repositories == null)
                _repositories = new ConcurrentDictionary<Type, object>();

            if (!_repositories.ContainsKey(typeof(TEntity)))
                _repositories[typeof(TEntity)] = new QueryRepositoryWithId<DbContext, TEntity, TId>(_context);

            return (IQueryRepositoryWithId<TEntity, TId>)_repositories[typeof(TEntity)];
        }
    }

    public class QueryRepository<TEntity> : QueryRepository<DbContext, TEntity>
        where TEntity : class, IAggregateRoot
    {
        public QueryRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }

    public class QueryRepository<TDbContext, TEntity> : QueryRepositoryWithId<TDbContext, TEntity, Guid>, IQueryRepository<TEntity>
        where TDbContext : DbContext
        where TEntity : class, IAggregateRoot
    {
        public QueryRepository(TDbContext dbContext) : base(dbContext)
        {
        }
    }

    /// <summary>
    /// Reference at https://github.com/ardalis/CachedRepository
    /// </summary>
    public class CachedQueryRepository<TEntity> : IQueryRepository<TEntity>
        where TEntity : class, IAggregateRoot
    {
        private readonly QueryRepository<TEntity> _queryRepository;
        private readonly IMemoryCache _cache;
        private readonly MemoryCacheEntryOptions _cacheOptions;

        public CachedQueryRepository(QueryRepository<TEntity> queryRepository, IMemoryCache cache, IConfiguration config)
        {
            var cacheSection = config.GetSection("EfCore:Cache");
            _queryRepository = queryRepository;
            _cache = cache;
            _cacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(relative: TimeSpan.FromSeconds(cacheSection.GetValue("ExpiredTime", 60)));
        }

        public IQueryable<TEntity> Queryable()
        {
            var key = $"queryable-{typeof(TEntity).Name.ToLowerInvariant()}";
            return _cache.GetOrCreate(key, entry =>
            {
                entry.SetOptions(_cacheOptions);
                return _queryRepository.Queryable();
            });
        }
    }

    public class QueryRepositoryWithId<TDbContext, TEntity, TId> : IQueryRepositoryWithId<TEntity, TId>
        where TDbContext : DbContext
        where TEntity : class, IAggregateRootWithId<TId>
    {
        private readonly TDbContext _dbContext;

        public QueryRepositoryWithId(TDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<TEntity> Queryable()
        {
            return _dbContext.Set<TEntity>();
        }
    }
}
