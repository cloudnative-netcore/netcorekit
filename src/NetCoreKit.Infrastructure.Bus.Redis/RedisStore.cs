using System;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace NetCoreKit.Infrastructure.Bus.Redis
{
  public class RedisStore
  {
    private static Lazy<ConnectionMultiplexer> _lazyConnection;

    public RedisStore(IOptions<RedisOptions> redisOptions)
    {
      var configurationOptions = new ConfigurationOptions
      {
        EndPoints =
        {
          redisOptions.Value.Connection
        },
        Password = redisOptions.Value.Password
      };

      _lazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(configurationOptions));
    }

    public ConnectionMultiplexer Connection => _lazyConnection.Value;

    public IDatabase RedisCache => Connection.GetDatabase();
  }

  public class RedisOptions
  {
    public string Connection { get; set; } = "127.0.0.1:6379";
    public string Password { get; set; } = "letmein";
  }
}
