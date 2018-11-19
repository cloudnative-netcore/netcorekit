using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NetCoreKit.Domain;

namespace NetCoreKit.Samples.BiMonetaryApi.Controllers
{
  public class Ticker : AggregateRootBase
  {
    public string Name { get; set; }
    public string Symbol { get; set; }
    public int Rank { get; set; }
    public double PriceUsd { get; private set; }
    public double PriceBtc { get; private set; }
    public double Volumn24HUsd { get; private set; }
    public double MarketCapUsd { get; private set; }
    public double AvailableSupply { get; private set; }
    public double TotalSupply { get; private set; }
    public string PercentChange1H { get; private set; }
    public string PercentChange24H { get; private set; }
    public string PercentChange7D { get; private set; }
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
    public ActionResult<IEnumerable<Ticker>> Get()
    {
      var repo = _repositoryFactory.QueryRepository<Ticker>();
      var tickers = repo.Queryable().ToList();
      return tickers;
    }

    // GET api/tickers/{C35A5A15-0913-43A0-BAD5-00FE9749C320}
    [HttpGet("{id:guid}")]
    public ActionResult<Ticker> Get(Guid id)
    {
      var repo = _repositoryFactory.QueryRepository<Ticker>();
      var ticker = repo.Queryable().FirstOrDefault(x => x.Id == id);
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
