using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
      try
      {
        foreach (var externalSystem in _externalSystems)
        {
          await externalSystem.Connect();
        }
      }
      catch (Exception)
      {
        return new BadRequestResult();
      }

      return Ok();
    }
  }
}
