using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace NetCoreKit.Infrastructure.EfCore.Migration
{
    public abstract class SeedDataBase<TDbContext> : IAuthConfigSeedData<TDbContext>
        where TDbContext : DbContext
    {
        protected SeedDataBase(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected IConfiguration Configuration { get; }

        public abstract Task SeedAsync(TDbContext context);
    }
}
