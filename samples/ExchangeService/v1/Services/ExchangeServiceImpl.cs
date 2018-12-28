using System;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using NetCoreKit.Samples.BiMonetaryApi.Rpc;

namespace NetCoreKit.Samples.ExchangeService.v1.Services
{
    public class ExchangeServiceImpl : BiMonetaryApi.Rpc.ExchangeService.ExchangeServiceBase
    {
        private readonly ILogger<ExchangeServiceImpl> _logger;

        public ExchangeServiceImpl(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ExchangeServiceImpl>();
        }

        public override Task<TokenResponse> GetTokenInfo(TokenRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"Start to process get extra info for {request.Symbol}.");

            var rnd = new Random();

            // TODO: just for demo
            var result = new TokenResponse
            {
                Rank = rnd.Next(1, 1000),
                PriceBtc = rnd.NextDouble() * 9999D + 1,
                PriceUsd = rnd.NextDouble() * 9999D + 1,
                MarketCapUsd = rnd.NextDouble() * 9999D + 1,
                AvailableSupply = rnd.NextDouble() * 9999D + 1,
                TotalSupply = rnd.NextDouble() * 9999D + 1,
                Volumn24HUsd = rnd.NextDouble() * 9999D + 1,
                PercentChange1H = rnd.Next(1, 100) + "%",
                PercentChange24H = rnd.Next(1, 100) + "%",
                PercentChange7D = rnd.Next(1, 100) + "%"
            };

            _logger.LogInformation("Return to caller.");

            return Task.FromResult(result);
        }

        public override Task<PingResponse> Ping(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new PingResponse {Result = true});
        }
    }
}
