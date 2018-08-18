using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NetCoreKit.Infrastructure.EfCore.Db;
using NetCoreKit.Infrastructure.EfCore.SqlServer.Options;

namespace NetCoreKit.Infrastructure.EfCore.SqlServer
{
  public static class ServiceCollectionExtensions
  {
    public static IServiceCollection AddEfCoreSqlServer(this IServiceCollection services)
    {
      var svcProvider = services.BuildServiceProvider();
      var config = svcProvider.GetRequiredService<IConfiguration>();

      services.Configure<MsSqlDbOptions>(config.GetSection("k8s:mssqldb"));

      services.Replace(
        ServiceDescriptor.Scoped<
          IDatabaseConnectionStringFactory,
          SqlServerDatabaseConnectionStringFactory>());

      services.Replace(
        ServiceDescriptor.Scoped<
          IExtendDbContextOptionsBuilder,
          SqlServerDbContextOptionsBuilderFactory>());

      return services;
    }
  }
}
