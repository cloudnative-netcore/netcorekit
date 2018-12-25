using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace NetCoreKit.Samples.TodoAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseDefaultServiceProvider(o => o.ValidateScopes = false /* because of MySQL */);
        }
    }
}
