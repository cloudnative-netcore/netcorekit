using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NetCoreKit.Infrastructure.AspNetCore.Miniservice;
using NetCoreKit.Infrastructure.EfCore.SqlServer;
using NetCoreKit.Samples.TodoAPI.Infrastructure.Db;

namespace NetCoreKit.Samples.TodoAPI
{
  public class Startup
	{
		public void ConfigureServices(IServiceCollection services)
		{
			var assemblies = new HashSet<Assembly>
			{
				typeof(Startup).GetTypeInfo().Assembly,
				typeof(MiniServiceExtensions).GetTypeInfo().Assembly
			};

			var serviceParams = new ServiceParams
			{
				{"assemblies", assemblies}
			};

			services.AddScoped(sp => serviceParams);
			services.AddEfCoreSqlServer();
			services.AddMiniService<TodoDbContext>();
		  services.AddExternalSystemHealthChecks();
		}

		public void Configure(IApplicationBuilder app)
		{
			app.UseMiniService();
		}
	}
}
