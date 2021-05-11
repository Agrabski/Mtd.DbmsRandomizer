using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Mtd.DbmsRandomizer.DatabaseManagement;

namespace TestApplication
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var app = CreateHostBuilder(args).Build();
			await app.Services.GetRequiredService<IDatabaseManager>().StartAsync();
			await app.RunAsync();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});
	}
}
