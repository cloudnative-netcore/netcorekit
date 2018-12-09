using System;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreKit.Domain
{
  public interface IUnitOfWorkAsync : IRepositoryAsyncFactory, IDisposable
  {
    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
  }
}
