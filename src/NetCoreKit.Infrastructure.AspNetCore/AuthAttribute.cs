using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace NetCoreKit.Infrastructure.AspNetCore
{
  public class AuthAttribute : AuthorizeAttribute
  {
    public AuthAttribute(string policy = null)
    {
      AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme;
      Policy = policy;
    }
  }
}
