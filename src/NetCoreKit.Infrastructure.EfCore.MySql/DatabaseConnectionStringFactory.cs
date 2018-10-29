using System.Linq;
using Microsoft.Extensions.Configuration;
using NetCoreKit.Infrastructure.EfCore.Db;

namespace NetCoreKit.Infrastructure.EfCore.MySql
{
  public sealed class DatabaseConnectionStringFactory : IDatabaseConnectionStringFactory
  {
    private readonly IConfiguration _config;

    public DatabaseConnectionStringFactory(IConfiguration config)
    {
      _config = config;
    }

    public string Create()
    {
      var connPattern = _config.GetConnectionString("MySqlDb");
      var connConfigs = _config.GetValue<string>("MySqlDb:FQDN").Split(':');
      var fqdn = connConfigs.First();
      var port = connConfigs.Except(new[] {fqdn}).First();

      return string.Format(
        connPattern,
        fqdn, port,
        _config.GetValue<string>("MySqlDb:UserName"),
        _config.GetValue<string>("MySqlDb:Password"),
        _config.GetValue<string>("MySqlDb:Database"));
    }
  }
}
