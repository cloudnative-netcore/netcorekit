using System;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using NetCoreKit.Domain;

namespace NetCoreKit.Infrastructure.Mongo
{
    public class MongoRepositoryAsync<TEntity> : IMongoQueryRepository<TEntity>, IRepositoryAsync<TEntity>
        where TEntity : IAggregateRoot
    {
        public MongoRepositoryAsync(MongoContext dbContext)
        {
            DbContext = dbContext;
        }

        public MongoContext DbContext { get; }

        public IQueryable<TEntity> Queryable()
        {
            var result = DbContext
                .Collection<TEntity>()
                .Find(_ => true)
                .ToListAsync();
            // we do a synchronous context here
            return result.Result.AsQueryable();
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            await DbContext.Collection<TEntity>().InsertOneAsync(entity);
            return await FindOneAsync(entity.Id);
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            await DbContext
                .Collection<TEntity>()
                .ReplaceOneAsync(n => n.Id.Equals(entity.Id), entity, new UpdateOptions {IsUpsert = true});
            return await FindOneAsync(entity.Id);
        }

        public async Task<TEntity> DeleteAsync(TEntity entity)
        {
            await DbContext
                .Collection<TEntity>()
                .DeleteOneAsync(Builders<TEntity>.Filter.Eq("Id", entity.Id));
            return await FindOneAsync(entity.Id);
        }

        private async Task<TEntity> FindOneAsync(Guid id)
        {
            var filter = Builders<TEntity>.Filter.Eq("Id", id);

            return await DbContext
                .Collection<TEntity>()
                .Find(filter)
                .FirstOrDefaultAsync();
        }
    }
}
