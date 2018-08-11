using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

      services.AddScoped<IExtendDbContextOptionsBuilder, SqlServerDbContextOptionsBuilderFactory>();
      services.AddScoped<IDatabaseConnectionStringFactory, SqlServerDatabaseConnectionStringFactory>();

      return services;
    }
  }
}
