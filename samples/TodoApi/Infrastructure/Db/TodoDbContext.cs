using Microsoft.EntityFrameworkCore;
using NetCoreKit.Infrastructure.EfCore.Db;

namespace NetCoreKit.Samples.TodoAPI.Infrastructure.Db
{
	public class TodoDbContext : ApplicationDbContext
	{
		public TodoDbContext(DbContextOptions options) : base(options)
		{
		}
	}
}