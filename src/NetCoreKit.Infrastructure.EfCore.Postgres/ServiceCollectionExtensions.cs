using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NetCoreKit.Infrastructure.EfCore.Db;

namespace NetCoreKit.Infrastructure.EfCore.Postgres
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEfCorePostgresDb(this IServiceCollection services)
        {
            var svcProvider = services.BuildServiceProvider();
            var config = svcProvider.GetRequiredService<IConfiguration>();

            services.Configure<DbOptions>(config.GetSection("Features:EfCore:PostgresDb"));

            services.Replace(
                ServiceDescriptor.Scoped<
                    IDbConnStringFactory,
                    DbConnStringFactory>());

            services.Replace(
                ServiceDescriptor.Scoped<
                    IExtendDbContextOptionsBuilder,
                    DbContextOptionsBuilderFactory>());

            return services;
        }
    }
}
