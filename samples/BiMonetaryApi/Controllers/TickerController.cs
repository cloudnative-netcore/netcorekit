using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NetCoreKit.Domain;
using NetCoreKit.Infrastructure.Mongo;

namespace NetCoreKit.Samples.BiMonetaryApi.Controllers
{
  public class Ticker : AggregateRootBase
  {
    public string Name { get; set; }
    public string Symbol { get; set; }
    public int Rank { get; set; }
    public double PriceUsd { get; private set; }
    public double PriceBtc { get; private set; }
    public double Volumn24hUsd { get; private set; }
    public double MarketCapUsd { get; private set; }
    public double AvailableSupply { get; private set; }
    public double TotalSupply { get; private set; }
    public string PercentChange1h { get; private set; }
    public string PercentChange24h { get; private set; }
    public string PercentChange7d { get; private set; }
    public DateTime LastSyncWithService { get; private set; }
    public string Link { get; private set; }
  }

  [Route("api/tickers")]
  [ApiController]
  public class TickerController : ControllerBase
  {
    private readonly IQueryRepositoryFactory _repositoryFactory;
    private readonly IUnitOfWorkAsync _uow;

    public TickerController(IQueryRepositoryFactory repositoryFactory, IUnitOfWorkAsync uow)
    {
      _repositoryFactory = repositoryFactory;
      _uow = uow;
    }

    // GET api/tickers
    [HttpGet]
    public async Task<ActionResult<PaginatedItem<Ticker>>> Get([FromQuery] Criterion criterion)
    {
      var repo = _repositoryFactory.MongoQueryRepository<Ticker>();
      var tickers = await repo.QueryAsync(criterion, ticker => ticker);
      return tickers;
    }

    // GET api/tickers/{C35A5A15-0913-43A0-BAD5-00FE9749C320}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Ticker>> Get(Guid id)
    {
      var repo = _repositoryFactory.MongoQueryRepository<Ticker>();
      var ticker = await repo.FindOneAsync(x => x.Id, id);
      return ticker;
    }

    // GET api/tickers/name/ETH
    [HttpGet("{name}")]
    public async Task<ActionResult<Ticker>> GetByName(string name)
    {
      var repo = _repositoryFactory.MongoQueryRepository<Ticker>();
      var ticker = await repo.FindOneAsync(x => x.Name, name);
      return ticker;
    }

    // POST api/tickers
    [HttpPost]
    public void Post([FromBody] Ticker ticker)
    {
      var repo = _uow.RepositoryAsync<Ticker>();
      repo.AddAsync(ticker);
    }
  }
}
