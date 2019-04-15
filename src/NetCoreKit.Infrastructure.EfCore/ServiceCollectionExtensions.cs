using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetCoreKit.Domain;
using NetCoreKit.Infrastructure.EfCore.Db;

namespace NetCoreKit.Infrastructure.EfCore
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGenericRepository(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWorkAsync, UnitOfWork>();
            services.AddScoped<IQueryRepositoryFactory, QueryRepositoryFactory>();
            return services;
        }

        public static IServiceCollection AddInMemoryDb(this IServiceCollection services)
        {
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
            return optionsBuilder.UseInMemoryDatabase("default_db");
        }
    }
}
