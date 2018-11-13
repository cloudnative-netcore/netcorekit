using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NetCoreKit.Infrastructure.EfCore.Db;
using NetCoreKit.Infrastructure.EfCore.MySql.Options;

namespace NetCoreKit.Infrastructure.EfCore.MySql
{
  public sealed class DbContextOptionsBuilderFactory : IExtendDbContextOptionsBuilder
  {
    private readonly DbOptions _options;

    public DbContextOptionsBuilderFactory(IOptions<DbOptions> options)
    {
      _options = options.Value ?? new DbOptions();
    }

    public DbContextOptionsBuilder Extend(
      DbContextOptionsBuilder optionsBuilder,
      IDatabaseConnectionStringFactory connectionStringFactory,
      string assemblyName)
    {
      return optionsBuilder.UseMySql(
          connectionStringFactory.Create(),
          sqlOptions =>
          {
            sqlOptions.MigrationsAssembly(assemblyName);
            sqlOptions.ServerVersion(_options.DbInfo);
            sqlOptions.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
          })
        .EnableSensitiveDataLogging();
    }
  }
}
