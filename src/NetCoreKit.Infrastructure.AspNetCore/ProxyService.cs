using NetCoreKit.Infrastructure.AspNetCore.Rest;

namespace NetCoreKit.Infrastructure.AspNetCore
{
  public abstract class ProxyServiceBase
  {
    protected readonly RestClient RestClient;

    protected ProxyServiceBase(RestClient rest)
    {
      RestClient = rest;
    }
  }
}
