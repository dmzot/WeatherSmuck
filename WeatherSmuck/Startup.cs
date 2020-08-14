using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WeatherSmuck.Hubs;
using WeatherSmuck.Models;

namespace WeatherSmuck
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			WeatherHub.Configuration = configuration;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.Configure<RouteOptions>(options =>
			{
				options.LowercaseUrls = true;
				options.LowercaseQueryStrings = true;
			});

			services.AddRazorPages((options) =>
			{
				options.Conventions.AllowAnonymousToPage("/Index");
			});

			services.AddSignalR();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			
			app.UseStaticFiles();

			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapHub<WeatherHub>("weather",
					(options) => { options.Transports = HttpTransportType.WebSockets; });

				endpoints.MapDefaultControllerRoute();
				endpoints.MapRazorPages();
			});
		}
	}
}
