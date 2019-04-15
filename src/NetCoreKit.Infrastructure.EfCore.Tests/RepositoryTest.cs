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
    public class TestEntity : AggregateRootBase
    {
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
            var testCommandRepo = uow.RepositoryAsync<TestEntity>();
            await testCommandRepo.AddAsync(new TestEntity());
            await testCommandRepo.AddAsync(new TestEntity());
            var result = await uow.SaveChangesAsync(default);

            // assert
            Assert.True(result >= 0);
        }

        [Fact]
        public async Task CanQueryOnGenericRepo()
        {
            // query
            var repoFactory = _services.BuildServiceProvider().GetService<IQueryRepositoryFactory>();
            var testQueryRepo = repoFactory.QueryRepository<TestEntity>();
            var result = await testQueryRepo.ListAsync<TestDbContext, TestEntity>();

            // assert
            Assert.NotNull(result.ToList());
        }
    }
}
