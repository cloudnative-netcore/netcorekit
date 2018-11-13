using System;
using System.Threading.Tasks;

namespace NetCoreKit.Infrastructure.AspNetCore.Miniservice.ExternalSystems
{
  [Obsolete]
  public class DbHealthCheckAndMigration : IExternalSystem
  {
    private readonly IServiceProvider _svcProvider;

    public DbHealthCheckAndMigration(IServiceProvider svcProvider)
    {
      _svcProvider = svcProvider;
    }

    public Task<bool> Connect()
    {
      return Task.Factory.StartNew(() => true);
    }
  }
}
