using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using WeatherSmuck.Models;

namespace WeatherSmuck
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var configFilename = "NLog.config";
			var logger = NLog.Web.NLogBuilder.ConfigureNLog(configFilename).GetCurrentClassLogger();
			NLog.LogManager.Configuration = logger.Factory.Configuration;

			try
			{
				CreateHostBuilder(args).Build().Run();
			}
			catch (Exception exception)
			{
				logger.Error(exception, "Stopped program because of exception");
				throw;
			}
			finally
			{
				NLog.LogManager.Shutdown();
			}
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
				.ConfigureLogging((builder => { builder.ClearProviders(); })).UseNLog();
	}
}
