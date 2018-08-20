using System;
using System.Threading.Tasks;
using NetCoreKit.Samples.TodoAPI.Domain;
using NetCoreKit.Samples.TodoAPI.Dtos;
using NetCoreKit.Utils.Attributes;
using Task = System.Threading.Tasks.Task;

namespace NetCoreKit.Samples.TodoAPI.Infrastructure.Gateways
{
  [AutoScanAwareness]
  public class UserGateway : IUserGateway
  {
    public Task<AuthorDto> GetAuthorAsync()
    {
      return Task.FromResult(
        new AuthorDto
        {
          Id = new Guid("E8F0B717-E325-466B-A87C-1AF1AA951599"),
          FirstName = "Tom",
          LastName = "Cruise"
        });
    }
  }
}
