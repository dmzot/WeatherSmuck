using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace WeatherSmuck.Models
{
	public class WeatherResponse
	{
		/// <summary>
		/// Response from Weather Provider
		/// </summary>
		public string weather { get; set; }

		/// <summary>
		/// Error message
		/// </summary>
		public string errorMessage { get; set; }
	}
}
