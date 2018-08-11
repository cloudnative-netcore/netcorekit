using System;
using Microsoft.AspNetCore.Identity;
using NetCoreKit.Domain;

namespace NetCoreKit.Infrastructure.EfCore.Identity
{
  public class ApplicationUser : IdentityUser<Guid>, IEntity
  {
    public string LastName
    {
      get;
      set;
    }

    public string FirstName
    {
      get;
      set;
    }
  }
}
