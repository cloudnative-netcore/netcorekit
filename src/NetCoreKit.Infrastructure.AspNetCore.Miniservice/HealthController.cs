using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NetCoreKit.Infrastructure.AspNetCore.Miniservice
{
  [Route("")]
  [ApiVersionNeutral]
  [ApiExplorerSettings(IgnoreApi = true)]
  public class HealthController : Controller
  {
    private readonly IEnumerable<IExternalSystem> _externalSystems;

    public HealthController(IEnumerable<IExternalSystem> externalSystems)
    {
      _externalSystems = externalSystems;
    }

    [HttpGet("/healthz")]
    public async Task<ActionResult> Get()
    {
      if(_externalSystems == null || !_externalSystems.Any())
        return new NoContentResult();

      try
      {
        var tasks = _externalSystems.Select(externalSystem => externalSystem.Connect()).Cast<Task>().ToList();
        await Task.WhenAll(tasks);
      }
      catch
      {
        return new StatusCodeResult(StatusCodes.Status503ServiceUnavailable);
      }

      return new NoContentResult();
    }
  }
}
