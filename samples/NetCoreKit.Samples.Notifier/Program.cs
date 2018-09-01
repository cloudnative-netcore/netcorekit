using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NetCoreKit.Samples.Notifier
{
  class Program
  {
    public static IConfiguration Configuration { get; set; }

    static void Main(string[] args)
    {
      Console.WriteLine("Starting application...");

      var configBuilder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json");

      var config = configBuilder.Build();

      var services = new ServiceCollection();

      var resolver = services.BuildServiceProvider();

      using (var scope = resolver.CreateScope())
      {
        var app = scope.ServiceProvider.GetService<IApplication>();
        app.Run();
      }

      Console.WriteLine("Started application.");
      Console.WriteLine("Waiting for messages...");
      Console.Read();
    }
  }
}
