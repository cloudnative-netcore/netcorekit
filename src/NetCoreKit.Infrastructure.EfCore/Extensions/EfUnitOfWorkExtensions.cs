using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetCoreKit.Domain;

namespace NetCoreKit.Infrastructure.EfCore.Extensions
{
    public static class EfUnitOfWorkExtensions
    {
        public static int? GetCommandTimeout(this IUnitOfWorkAsync uow, DbContext context)
        {
            return context.Database.GetCommandTimeout();
        }

        public static IUnitOfWorkAsync SetCommandTimeout(this IUnitOfWorkAsync uow, DbContext context, int? value)
        {
            context.Database.SetCommandTimeout(value);
            return uow;
        }

        public static int ExecuteSqlCommand(this IUnitOfWorkAsync uow, DbContext context, string sql,
            params object[] parameters)
        {
            return context.Database.ExecuteSqlCommand(sql, parameters);
        }

        public static async Task<int> ExecuteSqlCommandAsync(this IUnitOfWorkAsync uow, DbContext context, string sql,
            params object[] parameters)
        {
            return await context.Database.ExecuteSqlCommandAsync(sql, parameters);
        }

        public static async Task<int> ExecuteSqlCommandAsync(this IUnitOfWorkAsync uow, DbContext context, string sql,
            CancellationToken cancellationToken, params object[] parameters)
        {
            return await context.Database.ExecuteSqlCommandAsync(sql, cancellationToken, parameters);
        }
    }
}
