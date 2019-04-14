using System;
using Microsoft.EntityFrameworkCore;
using NetCoreKit.Infrastructure.EfCore.Db;

namespace NetCoreKit.Infrastructure.EfCore.SqlServer
{
    public sealed class DbContextOptionsBuilderFactory : IExtendDbContextOptionsBuilder
    {
        public DbContextOptionsBuilder Extend(
            DbContextOptionsBuilder optionsBuilder,
            IDbConnStringFactory connectionStringFactory,
            string assemblyName)
        {
            return optionsBuilder.UseSqlServer(
                    connectionStringFactory.Create(),
                    sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(assemblyName);
                        sqlOptions.EnableRetryOnFailure(
                            15,
                            TimeSpan.FromSeconds(30),
                            null);
                    })
                .EnableSensitiveDataLogging();
        }
    }
}
