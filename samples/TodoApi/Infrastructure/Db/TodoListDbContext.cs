using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NetCoreKit.Domain;
using NetCoreKit.Infrastructure.EfCore.Db;

namespace NetCoreKit.Samples.TodoAPI.Infrastructure.Db
{
    public class TodoListDbContext : AppDbContext
    {
        public TodoListDbContext(DbContextOptions<TodoListDbContext> options, IConfiguration config, IDomainEventDispatcher eventBus)
            : base(options, config, eventBus)
        {
        }
    }
}
