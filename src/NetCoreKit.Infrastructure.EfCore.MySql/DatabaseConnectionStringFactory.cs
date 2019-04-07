using System.Linq;
using Microsoft.Extensions.Options;
using NetCoreKit.Infrastructure.EfCore.Db;

namespace NetCoreKit.Infrastructure.EfCore.MySql
{
    public sealed class DatabaseConnectionStringFactory : IDatabaseConnectionStringFactory
    {
        private readonly DbOptions _dbOption;

        public DatabaseConnectionStringFactory()
        {
            var config = ConfigurationHelper.GetConfiguration();
            var dbSection = config.GetSection("Features:EfCore:MySqlDb");
            _dbOption = new DbOptions {
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
            _dbOption = options.Value;
        }

        public string Create()
        {
            var connPattern = _dbOption.ConnString;
            var connConfigs = _dbOption.FQDN?.Split(':');
            var fqdn = connConfigs?.First();
            var port = connConfigs?.Except(new[] {fqdn}).First();

            return string.Format(
                connPattern,
                fqdn, port,
                _dbOption.UserName,
                _dbOption.Password,
                _dbOption.Database);
        }
    }
}
