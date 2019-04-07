using System.Linq;
using Microsoft.Extensions.Options;
using NetCoreKit.Infrastructure.EfCore.Db;

namespace NetCoreKit.Infrastructure.EfCore.MySql
{
    public sealed class DatabaseConnectionStringFactory : IDatabaseConnectionStringFactory
    {
        public DbOptions DbOptions { get; }

        public DatabaseConnectionStringFactory()
        {
            var config = ConfigurationHelper.GetConfiguration();
            var dbSection = config.GetSection("Features:EfCore:MySqlDb");
            DbOptions = new DbOptions {
                ConnString = dbSection["ConnString"],
                FQDN = dbSection["FQDN"],
                Database = dbSection["Database"],
                DbInfo = dbSection["DbInfo"],
                UserName = dbSection["UserName"],
                Password = dbSection["Password"]
            };
        }

        public DatabaseConnectionStringFactory(IOptions<DbOptions> options)
        {
            DbOptions = options.Value;
        }

        public string Create()
        {
            var connPattern = DbOptions.ConnString;
            var connConfigs = DbOptions.FQDN?.Split(':');
            var fqdn = connConfigs?.First();
            var port = connConfigs?.Except(new[] {fqdn}).First();

            return string.Format(
                connPattern,
                fqdn, port,
                DbOptions.UserName,
                DbOptions.Password,
                DbOptions.Database);
        }
    }
}
