using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NetCoreKit.Domain;
using NetCoreKit.Infrastructure.Mongo;
using NetCoreKit.Samples.ExchangeService.Rpc;
using MyExchangeService = NetCoreKit.Samples.ExchangeService.Rpc.ExchangeService;

namespace NetCoreKit.Samples.BiMonetaryApi.Controllers
{
    public class Ticker : AggregateRootBase
    {
        public string Name { get; set; }
        public string Symbol { get; set; }
        public int Rank { get; set; }
        public double PriceUsd { get; set; }
        public double PriceBtc { get; set; }
        public double Volumn24hUsd { get; set; }
        public double MarketCapUsd { get; set; }
        public double AvailableSupply { get; set; }
        public double TotalSupply { get; set; }
        public string PercentChange1h { get; set; }
        public string PercentChange24h { get; set; }
        public string PercentChange7d { get; set; }
        public DateTime LastSyncWithService { get; set; }
        public string Link { get; set; }
    }

    [Route("api/tickers")]
    [ApiController]
    public class TickerController : ControllerBase
    {
        private readonly IQueryRepositoryFactory _repositoryFactory;
        private readonly IUnitOfWorkAsync _uow;
        private readonly MyExchangeService.ExchangeServiceClient _exchangeServiceClient;

        public TickerController(
            IQueryRepositoryFactory repositoryFactory,
            IUnitOfWorkAsync uow,
            MyExchangeService.ExchangeServiceClient exchangeServiceClient)
        {
            _repositoryFactory = repositoryFactory;
            _uow = uow;
            _exchangeServiceClient = exchangeServiceClient;
        }

        // GET api/tickers
        [HttpGet]
        public async Task<ActionResult<PaginatedItem<Ticker>>> Get([FromQuery] Criterion criterion)
        {
            var repo = _repositoryFactory.MongoQueryRepository<Ticker>();
            var tickers = await repo.QueryAsync(criterion, ticker => ticker);

            if (tickers == null || tickers.Items.Count <= 0) return tickers;

            var items = tickers.Items.SelectMany(x =>
            {
                var r = MapTickerWithExtraInfo(x);
                return new[] {r.Result};
            });

            return new PaginatedItem<Ticker>(tickers.TotalItems, tickers.TotalPages, items.ToList());
        }

        // GET api/tickers/{C35A5A15-0913-43A0-BAD5-00FE9749C320}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Ticker>> Get(Guid id)
        {
            var repo = _repositoryFactory.MongoQueryRepository<Ticker>();
            var ticker = await repo.FindOneAsync(x => x.Id, id);
            return await MapTickerWithExtraInfo(ticker);
        }

        // GET api/tickers/name/ETH
        [HttpGet("{name}")]
        public async Task<ActionResult<Ticker>> GetByName(string name)
        {
            var repo = _repositoryFactory.MongoQueryRepository<Ticker>();
            var ticker = await repo.FindOneAsync(x => x.Name, name);
            return await MapTickerWithExtraInfo(ticker);
        }

        // POST api/tickers
        [HttpPost]
        public void Post([FromBody] Ticker ticker)
        {
            var repo = _uow.RepositoryAsync<Ticker>();
            repo.AddAsync(ticker);
        }

        private async Task<Ticker> MapTickerWithExtraInfo(Ticker ticker)
        {
            if (ticker == null) return ticker;
            var info = await _exchangeServiceClient.GetTokenInfoAsync(new TokenRequest {Symbol = ticker.Symbol});

            if (info == null)
            {
                return ticker;
            }

            ticker.Rank = info.Rank;
            ticker.PriceUsd = info.PriceUsd;
            ticker.PriceBtc = info.PriceBtc;
            ticker.Volumn24hUsd = info.Volumn24HUsd;
            ticker.MarketCapUsd = info.MarketCapUsd;
            ticker.AvailableSupply = info.AvailableSupply;
            ticker.TotalSupply = info.TotalSupply;
            ticker.PercentChange1h = info.PercentChange1H;
            ticker.PercentChange24h = info.PercentChange24H;
            ticker.PercentChange7d = info.PercentChange7D;

            return ticker;
        }
    }
}
