using NetCoreKit.Domain;
using NetCoreKit.Infrastructure.EfCore.Repository;

namespace NetCoreKit.Infrastructure.EfCore.Extensions
{
    public static class EfQueryRepositoryFactoryExtensions
    {
        public static IEfQueryRepository<TEntity> QueryEfRepository<TEntity>(this IQueryRepositoryFactory factory)
            where TEntity : IAggregateRoot
        {
            return factory.QueryRepository<TEntity>() as IEfQueryRepository<TEntity>;
        }
    }
}
