using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NetCoreKit.Infrastructure.EfCore.Db;
using NetCoreKit.Infrastructure.EfCore.MySql.Options;

namespace NetCoreKit.Infrastructure.EfCore.MySql
{
  public static class ServiceCollectionExtensions
  {
    public static IServiceCollection AddEfCoreMySqlDb(this IServiceCollection services)
    {
      var svcProvider = services.BuildServiceProvider();
      var config = svcProvider.GetRequiredService<IConfiguration>();

      services.Configure<DbOptions>(config.GetSection("k8s:mysqldb"));

      services.Replace(
        ServiceDescriptor.Scoped<
          IDatabaseConnectionStringFactory,
          DatabaseConnectionStringFactory>());

      services.Replace(
        ServiceDescriptor.Scoped<
          IExtendDbContextOptionsBuilder,
          DbContextOptionsBuilderFactory>());

      return services;
    }
  }
}
