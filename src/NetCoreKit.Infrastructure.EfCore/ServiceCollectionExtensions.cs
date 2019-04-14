using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCoreKit.Domain;
using NetCoreKit.Infrastructure.EfCore.Db;
using NetCoreKit.Infrastructure.EfCore.Repository;

namespace NetCoreKit.Infrastructure.EfCore
{
    public static class QueryRepositoryFactoryExtensions
    {
        public static IQueryRepository<TEntity> QueryEfRepository<TEntity>(this IQueryRepositoryFactory factory)
            where TEntity : IAggregateRoot
        {
            return factory.QueryRepository<TEntity>() as IQueryRepository<TEntity>;
        }

        public static IQueryRepositoryWithType<TEntity, TId> QueryEfRepository<TEntity, TId>(this IQueryRepositoryFactory factory)
            where TEntity : IAggregateRootWithType<TId>
        {
            return factory.QueryRepository<TEntity, TId>() as IQueryRepositoryWithType<TEntity, TId>;
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGenericRepository(this IServiceCollection services)
        {
            var svcProvider = services.BuildServiceProvider();

            var entityTypes = svcProvider
                .GetRequiredService<IConfiguration>()
                .LoadFullAssemblies()
                .SelectMany(m => m.DefinedTypes)
                .Where(x => typeof(IAggregateRoot).IsAssignableFrom(x) && !x.GetTypeInfo().IsAbstract);

            foreach (var entity in entityTypes)
            {
                var repoType = typeof(IRepositoryAsync<>).MakeGenericType(entity);
                var implRepoType = typeof(RepositoryAsync<,>).MakeGenericType(typeof(DbContext), entity);
                services.AddScoped(repoType, implRepoType);

                var queryRepoType = typeof(IQueryRepository<>).MakeGenericType(entity);
                var implQueryRepoType = typeof(QueryRepository<,>).MakeGenericType(typeof(DbContext), entity);
                services.AddScoped(queryRepoType, implQueryRepoType);
            }

            services.AddScoped<IUnitOfWorkAsync, UnitOfWork>();
            services.AddScoped<IQueryRepositoryFactory, QueryRepositoryFactory>();

            return services;
        }

        public static IServiceCollection AddEfSqlLiteDb(this IServiceCollection services)
        {
            // default if we don't declare any db provider
            services.AddScoped<IDbConnStringFactory, NoOpDbConnStringFactory>();
            services.AddScoped<IExtendDbContextOptionsBuilder, InMemoryDbContextOptionsBuilderFactory>();
            return services;
        }
    }

    internal class NoOpDbConnStringFactory : IDbConnStringFactory
    {
        public string Create()
        {
            return string.Empty;
        }
    }

    internal class InMemoryDbContextOptionsBuilderFactory : IExtendDbContextOptionsBuilder
    {
        public DbContextOptionsBuilder Extend(
            DbContextOptionsBuilder optionsBuilder,
            IDbConnStringFactory connStringFactory,
            string assemblyName)
        {
            return optionsBuilder.UseInMemoryDatabase("defaultDb");
        }
    }
}
