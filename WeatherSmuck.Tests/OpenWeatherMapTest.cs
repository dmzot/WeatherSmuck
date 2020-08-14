using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using WeatherSmuck.Models;
using Xunit;

namespace WeatherSmuck.Tests
{
	public class OpenWeatherMapTest
	{
		private IConfiguration Configuration;

		public OpenWeatherMapTest()
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(Path.GetFullPath(@"..\..\..\..\WeatherSmuck"))
				.AddJsonFile("appsettings.json");
			Configuration = builder.Build();
		}

		[Fact]
		public void Load_APIKey()
		{
			Assert.True(!string.IsNullOrEmpty(Configuration["OpenWeatherMapAPIKey"]));
		}

		[Fact]
		public async Task APIRequest()
		{
			var request = new WeatherRequest()
			{
				lon = (decimal)55.751999,
				lat = (decimal)37.617734
			};
			var provider = new OpenWeatherProvider(Configuration);
			var result = await provider.GetCurrentWeather(request);
			Assert.True(string.IsNullOrEmpty(result.errorMessage));
			Assert.Contains("id", result.weather);
		}
	}
}
