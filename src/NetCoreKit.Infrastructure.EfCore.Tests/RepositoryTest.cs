using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCoreKit.Domain;
using NetCoreKit.Infrastructure.EfCore.Db;
using NetCoreKit.Infrastructure.EfCore.Extensions;
using Xunit;

namespace NetCoreKit.Infrastructure.EfCore.Tests
{
    public class TestEvent : EventBase
    {
    }

    public class TestEntity : AggregateRootBase
    {
        public TestEntity()
        {
            AddEvent(new TestEvent());
        }
    }

    public class TestEntityWithId : AggregateRootWithIdBase<int>
    {
        public TestEntityWithId()
        {
            AddEvent(new TestEvent());
        }
    }

    public class TestDbContext : AppDbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options, IConfiguration config, IDomainEventDispatcher eventBus = null)
            : base(options, config, eventBus)
        {
        }
    }

    public class RepositoryTest
    {
        private readonly IServiceCollection _services;

        public RepositoryTest()
        {
            // arrange
            _services = new ServiceCollection();
            var config = ConfigurationHelper.GetConfiguration();
            _services.AddScoped<IConfiguration>(x => config);
            _services.AddDbContext<TestDbContext>((sp, o) =>
            {
                o.UseInMemoryDatabase("default_db");
            });

            _services.AddScoped<DbContext>(resolver => resolver.GetService<TestDbContext>());
            _services.AddGenericRepository();
        }

        [Fact]
        public async Task CanCommandOnGenericRepo()
        {
            // command
            var uow = _services.BuildServiceProvider().GetService<IUnitOfWorkAsync>();

            // without id type, default is Guid
            var testCommandRepo = uow.RepositoryAsync<TestEntity>();
            await testCommandRepo.AddAsync(new TestEntity());
            await testCommandRepo.AddAsync(new TestEntity());

            // with int type for id
            var testCommandRepoWithId = uow.RepositoryAsync<TestEntityWithId, int>();
            await testCommandRepoWithId.AddAsync(new TestEntityWithId());

            var result = await uow.SaveChangesAsync(default);

            // assert
            Assert.True(result >= 0);
        }

        [Fact]
        public async Task CanQueryOnGenericRepo()
        {
            // query
            var repoFactory = _services.BuildServiceProvider().GetService<IQueryRepositoryFactory>();

            // without id type, default is Guid
            var testQueryRepo = repoFactory.QueryRepository<TestEntity>();
            var result1 = await testQueryRepo.ListAsync<TestDbContext, TestEntity>();

            // with int type for id
            var testQueryRepoWithId = repoFactory.QueryRepository<TestEntityWithId, int>();
            var result2 = await testQueryRepoWithId.ListAsync<TestDbContext, TestEntityWithId, int>();

            // assert
            Assert.NotNull(result1.ToList());
            Assert.NotNull(result2.ToList());
        }
    }
}
