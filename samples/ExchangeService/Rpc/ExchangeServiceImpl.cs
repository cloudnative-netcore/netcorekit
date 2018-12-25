using System;
using System.Threading.Tasks;
using Grpc.Core;

namespace NetCoreKit.Samples.ExchangeService.Rpc
{
    public class ExchangeServiceImpl : ExchangeService.ExchangeServiceBase
    {
        public override Task<TokenResponse> GetTokenInfo(TokenRequest request, ServerCallContext context)
        {
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
                PercentChange1H = rnd.Next(1, 100) / 100 + "",
                PercentChange24H = rnd.Next(1, 100) / 100 + "",
                PercentChange7D = rnd.Next(1, 100) / 100 + ""
            };

            return Task.FromResult(result);
        }
    }
}
