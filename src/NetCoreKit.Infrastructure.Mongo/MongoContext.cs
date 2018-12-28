using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NetCoreKit.Domain;

namespace NetCoreKit.Infrastructure.Mongo
{
    public class MongoContext
    {
        private readonly IMongoDatabase _database;

        public MongoContext(IOptions<MongoSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnString);
            _database = client.GetDatabase(settings.Value.Database);
        }

        public IMongoCollection<TEntity> Collection<TEntity>()
            where TEntity : IAggregateRoot
        {
            return _database.GetCollection<TEntity>(typeof(TEntity).FullName);
        }
    }
}
