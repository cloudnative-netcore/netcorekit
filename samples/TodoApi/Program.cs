using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NetCoreKit.Infrastructure.EfCore.Extensions;
using NetCoreKit.Samples.TodoAPI.Infrastructure.Db;

namespace NetCoreKit.Samples.TodoAPI
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var webHost = CreateWebHostBuilder(args).Build();
      var env = webHost.Services.GetService<IHostingEnvironment>();
      if (env.IsDevelopment()) webHost = webHost.RegisterDbContext<TodoListDbContext>();
      webHost.Run();
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args)
    {
      return WebHost.CreateDefaultBuilder(args)
        .UseStartup<Startup>();
    }
  }
}
