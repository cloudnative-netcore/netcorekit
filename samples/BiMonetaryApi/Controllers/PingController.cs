using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using MyExchangeService = NetCoreKit.Samples.BiMonetaryApi.Rpc.ExchangeService;

namespace NetCoreKit.Samples.BiMonetaryApi.Controllers
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
        public IActionResult Ping()
        {
            return Ok(_exchangeServiceClient.PingAsync(new Empty()));
        }
    }
}
