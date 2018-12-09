using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NetCoreKit.Infrastructure.AspNetCore.Configuration;

namespace NetCoreKit.Infrastructure.AspNetCore.All.Controllers
{
  [Route("")]
  [ApiVersionNeutral]
  [ApiExplorerSettings(IgnoreApi = true)]
  public class HomeController : Controller
  {
    private readonly string _basePath;

    public HomeController(IConfiguration config)
    {
      _basePath = config.GetBasePath() ?? "/";
    }

    [HttpGet]
    public IActionResult Index()
    {
      return Redirect($"~{_basePath}swagger");
    }
  }
}
