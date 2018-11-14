using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCoreKit.Domain;

namespace NetCoreKit.Infrastructure.Mongo
{
  public static class ServiceCollectionExtensions
  {
    public static IServiceCollection AddMongoDb(this IServiceCollection services)
    {
      var svcProvider = services.BuildServiceProvider();
      var config = svcProvider.GetRequiredService<IConfiguration>();

      var entityTypes = config
        .LoadFullAssemblies()
        .SelectMany(m => m.DefinedTypes)
        .Where(x => typeof(IAggregateRoot).IsAssignableFrom(x) && !x.GetTypeInfo().IsAbstract);

      foreach (var entity in entityTypes)
      {
        var repoType = typeof(IRepositoryAsync<>).MakeGenericType(entity);
        var implRepoType = typeof(MongoRepositoryAsync<>).MakeGenericType(entity);
        services.AddScoped(repoType, implRepoType);

        var queryRepoType = typeof(IQueryRepository<>).MakeGenericType(entity);
        var implQueryRepoType = typeof(MongoQueryRepository<>).MakeGenericType(entity);
        services.AddScoped(queryRepoType, implQueryRepoType);
      }

      services.Configure<MongoSettings>(config.GetSection("Mongo"));
      services.AddScoped<MongoContext>();

      services.AddScoped<IUnitOfWorkAsync, MongoUnitOfWorkAsync>();
      services.AddScoped<IQueryRepositoryFactory, MongoQueryRepositoryFactory>();

      return services;
    }
  }
}
