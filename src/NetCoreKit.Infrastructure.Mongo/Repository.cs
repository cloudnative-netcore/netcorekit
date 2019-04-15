using System;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using NetCoreKit.Domain;

namespace NetCoreKit.Infrastructure.Mongo
{
    public class Repository<TEntity> : IMongoQueryRepository<TEntity>, IRepositoryAsync<TEntity>
        where TEntity : IAggregateRoot
    {
        private readonly IDomainEventDispatcher _domainEventDispatcher;

        public Repository(DbContext dbContext, IDomainEventDispatcher domainEventDispatcher)
        {
            DbContext = dbContext;
            _domainEventDispatcher = domainEventDispatcher;
        }

        public DbContext DbContext { get; }

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

            await PublishDomainEventAsync(entity);

            return await FindOneAsync(entity.Id);
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            await DbContext
                .Collection<TEntity>()
                .ReplaceOneAsync(n => n.Id.Equals(entity.Id), entity, new UpdateOptions {IsUpsert = true});

            await PublishDomainEventAsync(entity);

            return await FindOneAsync(entity.Id);
        }

        public async Task<TEntity> DeleteAsync(TEntity entity)
        {
            await DbContext
                .Collection<TEntity>()
                .DeleteOneAsync(Builders<TEntity>.Filter.Eq("Id", entity.Id));

            await PublishDomainEventAsync(entity);

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

        private async Task PublishDomainEventAsync(TEntity entity)
        {
            var events = entity.GetUncommittedEvents().ToArray();
            if (events.Length > 0)
                foreach (var domainEvent in events)
                    await _domainEventDispatcher.Dispatch(domainEvent);
        }
    }
}
