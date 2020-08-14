using System;
using Newtonsoft.Json;

namespace WeatherSmuck.Models
{
	public enum unitsEnum {metric, imperial}

	/// <summary>
	/// coordinates model
	/// </summary>
	public class WeatherRequest
	{
		/// <summary>
		/// longitude
		/// </summary>
		public decimal lon { get; set; }

		/// <summary>
		/// latitude
		/// </summary>
		public decimal lat { get; set; }

		/// <summary>
		/// Units (metric, imperial)
		/// </summary>
		public unitsEnum units { get; set; } = unitsEnum.metric;
	}
}
