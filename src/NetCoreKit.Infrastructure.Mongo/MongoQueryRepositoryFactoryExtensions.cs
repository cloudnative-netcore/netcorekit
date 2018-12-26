using NetCoreKit.Domain;

namespace NetCoreKit.Infrastructure.Mongo
{
    public static class MongoQueryRepositoryFactoryExtensions
    {
        public static IMongoQueryRepository<TEntity> MongoQueryRepository<TEntity>(this IQueryRepositoryFactory factory)
            where TEntity : IAggregateRoot
        {
            return factory.QueryRepository<TEntity>() as IMongoQueryRepository<TEntity>;
        }
    }
}
