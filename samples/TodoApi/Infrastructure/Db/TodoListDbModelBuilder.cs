using Microsoft.EntityFrameworkCore;
using NetCoreKit.Infrastructure.EfCore.Db;
using NetCoreKit.Samples.TodoAPI.Domain;

namespace NetCoreKit.Samples.TodoAPI.Infrastructure.Db
{
  public class TodoListDbModelBuilder : ICustomModelBuilder
  {
    public void Build(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Project>(b =>
      {
        b.HasMany(t => t.Tasks).WithOne(a => a.Project)
          .HasForeignKey(k => k.ProjectId)
          .OnDelete(DeleteBehavior.Cascade);
      });
    }
  }
}
