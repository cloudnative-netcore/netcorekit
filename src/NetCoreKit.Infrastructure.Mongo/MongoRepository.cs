using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NetCoreKit.Domain;

namespace NetCoreKit.Infrastructure.Mongo
{
  public class MongoQueryRepository<TEntity> : IQueryRepository<TEntity>
    where TEntity : IAggregateRoot
  {
    public MongoContext DbContext { get; }

    public MongoQueryRepository(IOptions<MongoSettings> settings)
    {
      DbContext = new MongoContext(settings);
    }

    public IQueryable<TEntity> Queryable()
    {
      var result = DbContext
        .Collection<TEntity>()
        .Find(_ => true)
        .ToListAsync();

      // we do a synchronous context here
      return result.Result.AsQueryable();
    }
  }

  public class MongoRepository<TEntity> : IRepositoryAsync<TEntity>
    where TEntity : IAggregateRoot
  {
    public MongoContext DbContext { get; }

    public MongoRepository(IOptions<MongoSettings> settings)
    {
      DbContext = new MongoContext(settings);
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
      await DbContext.Collection<TEntity>().InsertOneAsync(entity);
      return await GetByIdAsync(entity.Id);
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
      await DbContext
        .Collection<TEntity>()
        .ReplaceOneAsync(n => n.Id.Equals(entity.Id), entity, new UpdateOptions { IsUpsert = true });

      return await GetByIdAsync(entity.Id);
    }

    public async Task<TEntity> DeleteAsync(TEntity entity)
    {
      await DbContext
        .Collection<TEntity>()
        .DeleteOneAsync(Builders<TEntity>.Filter.Eq("Id", entity.Id));

      return await GetByIdAsync(entity.Id);
    }

    public async Task<TEntity> GetByIdAsync(Guid id)
    {
      var filter = Builders<TEntity>.Filter.Eq("Id", id);
      return await DbContext
        .Collection<TEntity>()
        .Find(filter)
        .FirstOrDefaultAsync();
    }
  } 
}
