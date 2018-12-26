using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NetCoreKit.Infrastructure.EfCore.Db;

namespace NetCoreKit.Infrastructure.EfCore.MySql
{
    public sealed class DatabaseConnectionStringFactory : IDatabaseConnectionStringFactory
    {
        private readonly IConfiguration _config;
        private readonly DbOptions _dbOption;

        public DatabaseConnectionStringFactory(IConfiguration config, IOptions<DbOptions> options)
        {
            _config = config;
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
