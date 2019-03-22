using NetCoreKit.Domain;

namespace NetCoreKit.Infrastructure.Mongo
{
    public interface IMongoQueryRepository<out TEntity> : IQueryRepository<TEntity>
        where TEntity : IAggregateRoot
    {
        MongoContext DbContext { get; }
    }
}
