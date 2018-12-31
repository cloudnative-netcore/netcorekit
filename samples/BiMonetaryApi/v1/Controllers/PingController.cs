using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using MyExchangeService = NetCoreKit.Samples.BiMonetaryApi.Rpc.ExchangeService;

namespace NetCoreKit.Samples.BiMonetaryApi.v1.Controllers
{
    [Route("api/ping")]
    [ApiController]
    public class PingController : ControllerBase
    {
        private readonly MyExchangeService.ExchangeServiceClient _exchangeServiceClient;

        public PingController(MyExchangeService.ExchangeServiceClient exchangeServiceClient)
        {
            _exchangeServiceClient = exchangeServiceClient;
        }

        [HttpGet]
        public async Task<IActionResult> Ping()
        {
            var response = await _exchangeServiceClient.PingAsync(new Empty());
            return Ok(response.Result);
        }
    }
}
