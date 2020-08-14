using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using NLog;
using WeatherSmuck.Models;

namespace WeatherSmuck.Hubs
{
	/// <summary>
	/// SignalR layer for communication
	/// </summary>
	public class WeatherHub : Hub
	{
		private static Logger Log = LogManager.GetCurrentClassLogger();

		public static IConfiguration Configuration;

		public OpenWeatherProvider Provider;

		public WeatherHub()
		{
			Provider = new OpenWeatherProvider(Configuration);
		}

		public async Task<WeatherResponse> GetCurrentWeather(WeatherRequest request)
		{
			try
			{
				return await Provider.GetCurrentWeather(request);
			}
			catch (Exception ex)
			{
				Log.Error(ex);
				return new WeatherResponse()
				{
					errorMessage = $"Internal server error. {ex.Message}"
				};
			}
		}
	}
}
