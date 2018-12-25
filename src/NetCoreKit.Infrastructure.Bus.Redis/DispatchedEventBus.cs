using System;
using System.Threading.Tasks;
using Google.Protobuf;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace NetCoreKit.Infrastructure.Bus.Redis
{
  public class DispatchedEventBus : IDispatchedEventBus
  {
    private readonly RedisStore _redisStore;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DispatchedEventBus> _logger;

    public DispatchedEventBus(RedisStore redisStore, IServiceProvider serviceProvider, ILoggerFactory factory)
    {
      _redisStore = redisStore;
      _serviceProvider = serviceProvider;
      _logger = factory.CreateLogger<DispatchedEventBus>();
    }

    public async Task PublishAsync<TMessage>(TMessage msg, params string[] topics) where TMessage : IMessage<TMessage>
    {
      var redis = _redisStore.RedisCache;
      var pub = redis.Multiplexer.GetSubscriber();

      foreach (var topic in topics)
      {
        _logger.LogInformation($"[NCK: {topic}] Publishing the message...");
        await pub.PublishAsync(topic, msg.ToByteString().ToByteArray());
      }
    }

    public async Task SubscribeAsync<TMessage>(params string[] topics) where TMessage : IMessage<TMessage>, new()
    {
      var redis = _redisStore.RedisCache;
      var sub = redis.Multiplexer.GetSubscriber();

      foreach (var topic in topics)
      {
        await sub.SubscribeAsync(topic, async (channel, message) =>
        {
          _logger.LogInformation($"[NCK: {topic}] Subscribing to the message...");
          var msg = (TMessage)Activator.CreateInstance(typeof(TMessage));
          msg.MergeFrom(message);
          var keyField = msg.Descriptor.FindFieldByName("Key");
          var key = keyField.Accessor.GetValue(msg);

          using (var scope = _serviceProvider.CreateScope())
          {
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var notify = msg.ToNotification(config, key.ToString());
            await mediator.Publish(notify);
          }
        });
      }
    }
  }
}
