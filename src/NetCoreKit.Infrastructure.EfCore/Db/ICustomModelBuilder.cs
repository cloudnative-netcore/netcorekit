using Microsoft.EntityFrameworkCore;

namespace NetCoreKit.Infrastructure.EfCore.Db
{
    public interface ICustomModelBuilder
    {
        void Build(ModelBuilder modelBuilder);
    }
}
