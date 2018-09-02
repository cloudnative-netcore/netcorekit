using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using NetCoreKit.Infrastructure.EfCore.Db;

namespace NetCoreKit.Infrastructure.EfCore.MySql
{
  public sealed class DatabaseConnectionStringFactory : IDatabaseConnectionStringFactory
  {
    private readonly IConfiguration _config;
    private readonly IHostingEnvironment _env;

    public DatabaseConnectionStringFactory(
      IConfiguration config,
      IHostingEnvironment env)
    {
      _config = config;
      _env = env;
    }

    public string Create()
    {
      if (_env.IsDevelopment())
      {
        return _config.GetConnectionString("mysqldb");
      }

      var serviceName = _config.GetValue<string>("k8s:mysqldb:ServiceName").ToUpperInvariant();
      return string.Format(
        _config.GetConnectionString("mysqldb"),
        Environment.GetEnvironmentVariable($"{serviceName}_SERVICE_HOST"),
        Environment.GetEnvironmentVariable($"{serviceName}_SERVICE_PORT"),
        _config.GetValue<string>("k8s:mysqldb:UserName"),
        _config.GetValue<string>("k8s:mysqldb:Password"),
        _config.GetValue<string>("k8s:mysqldb:Database"));
    }
  }
}
