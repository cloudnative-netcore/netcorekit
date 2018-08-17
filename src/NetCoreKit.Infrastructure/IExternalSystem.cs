using System.Threading.Tasks;

namespace NetCoreKit.Infrastructure
{
  public interface IExternalSystem
  {
    Task<bool> Connect();
  }
}
