using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NetCoreKit.Infrastructure.EfCore.Db;
using NetCoreKit.Infrastructure.EfCore.SqlServer.Options;

namespace NetCoreKit.Infrastructure.EfCore.SqlServer
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEfCoreSqlServerDb(this IServiceCollection services)
        {
            var svcProvider = services.BuildServiceProvider();
            var config = svcProvider.GetRequiredService<IConfiguration>();

            services.Configure<DbOptions>(config.GetSection("k8s:mssqldb"));

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
