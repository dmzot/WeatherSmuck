using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NLog.Fluent;

namespace WeatherSmuck.Models
{
	public class OpenWeatherProvider
	{
		private readonly string APIKey;

		private static HttpClient client = null;

		public OpenWeatherProvider(IConfiguration configuration)
		{
			APIKey = configuration["OpenWeatherMapAPIKey"];

			var handler = new HttpClientHandler()
			{
				AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
			};
			client = new HttpClient(handler)
			{
				BaseAddress = new Uri("https://api.openweathermap.org/data/2.5/")
			};
		}

		public async Task<WeatherResponse> GetCurrentWeather(WeatherRequest request)
		{
			var response = await client.GetAsync($"weather?lat={request.lat}&lon={request.lon}&units={request.units}&appid={APIKey}");
			if (response.StatusCode == HttpStatusCode.Unauthorized)
				throw new Exception("Please, check OpenWeatheMap API Key");
			return new WeatherResponse()
			{
				weather = response.Content.ReadAsStringAsync().Result
			};
		}
	}
}
