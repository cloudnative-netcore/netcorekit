using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using NetCoreKit.Domain;

namespace NetCoreKit.Infrastructure.Mongo
{
  public class MongoQueryRepositoryFactory : IQueryRepositoryFactory
  {
    private readonly IServiceProvider _serviceProvider;

    public MongoQueryRepositoryFactory(IServiceProvider serviceProvider)
    {
      _serviceProvider = serviceProvider;
    }

    public IQueryRepository<TEntity> QueryRepository<TEntity>() where TEntity : IAggregateRoot
    {
      return _serviceProvider.GetService(typeof(IQueryRepository<TEntity>)) as IQueryRepository<TEntity>;
    }
  }

  public class MongoUnitOfWorkAsync : IUnitOfWorkAsync
  {
    private ConcurrentDictionary<Type, object> _repositories;
    private readonly MongoContext _dbContext;

    public MongoUnitOfWorkAsync(MongoContext dbContext)
    {
      _dbContext = dbContext;
    }

    public virtual IRepositoryAsync<TEntity> RepositoryAsync<TEntity>() where TEntity : class, IAggregateRoot
    {
      if (_repositories == null)
      {
        _repositories = new ConcurrentDictionary<Type, object>();
      }
      var type = typeof(TEntity);
      if (!_repositories.ContainsKey(type))
      {
        _repositories[type] = new MongoRepositoryAsync<TEntity>(_dbContext);
      }

      return (IRepositoryAsync<TEntity>)_repositories[type];
    }

    public IRepositoryAsync<TEntity> MongoRepository<TEntity>() where TEntity : class, IAggregateRoot
    {
      if (_repositories == null)
      {
        _repositories = new ConcurrentDictionary<Type, object>();
      }
      var type = typeof(TEntity);
      if (!_repositories.ContainsKey(type))
      {
        _repositories[type] = new MongoRepositoryAsync<TEntity>(_dbContext);
      }

      return (IRepositoryAsync<TEntity>)_repositories[type];
    }

    public void Dispose()
    {
      if(_dbContext != null) GC.SuppressFinalize(_dbContext);
    }

    public int SaveChanges()
    {
      throw new NotImplementedException();
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }
  }

  public class MongoQueryRepository<TEntity> : IQueryRepository<TEntity>
    where TEntity : IAggregateRoot
  {
    private readonly MongoContext _dbContext;

    public MongoQueryRepository(MongoContext dbContext)
    {
      _dbContext = dbContext;
    }

    public IQueryable<TEntity> Queryable()
    {
      var result = _dbContext
        .Collection<TEntity>()
        .Find(_ => true)
        .ToListAsync();

      // we do a synchronous context here
      return result.Result.AsQueryable();
    }
  }

  public class MongoRepositoryAsync<TEntity> : IRepositoryAsync<TEntity>
    where TEntity : IAggregateRoot
  {
    private readonly MongoContext _dbContext;

    public MongoRepositoryAsync(MongoContext dbContext)
    {
      _dbContext = dbContext;
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
      await _dbContext.Collection<TEntity>().InsertOneAsync(entity);
      return await GetByIdAsync(entity.Id);
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
      await _dbContext
        .Collection<TEntity>()
        .ReplaceOneAsync(n => n.Id.Equals(entity.Id), entity, new UpdateOptions { IsUpsert = true });

      return await GetByIdAsync(entity.Id);
    }

    public async Task<TEntity> DeleteAsync(TEntity entity)
    {
      await _dbContext
        .Collection<TEntity>()
        .DeleteOneAsync(Builders<TEntity>.Filter.Eq("Id", entity.Id));

      return await GetByIdAsync(entity.Id);
    }

    public async Task<TEntity> GetByIdAsync(Guid id)
    {
      var filter = Builders<TEntity>.Filter.Eq("Id", id);

      return await _dbContext
        .Collection<TEntity>()
        .Find(filter)
        .FirstOrDefaultAsync();
    }
  } 
}
