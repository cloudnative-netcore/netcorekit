using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NetCoreKit.Infrastructure.EfCore.Db;

namespace NetCoreKit.Infrastructure.EfCore.Postgres
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
            IDbConnStringFactory connStringFactory,
            string assemblyName)
        {
            return optionsBuilder.UseNpgsql(
                    connStringFactory.Create(),
                    sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(assemblyName);
                        sqlOptions.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
                    })
                .EnableSensitiveDataLogging();
        }
    }
}
