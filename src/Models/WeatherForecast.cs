using System;

namespace Weathy.Models
{
    /// <summary>
    /// Weather forecast
    /// </summary>
    public class WeatherForecast
    {
        /// <summary>
        /// Date and time
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Temperature in Celsius degrees
        /// </summary>
        public int TemperatureC { get; set; }

        /// <summary>
        /// Temperature in Fahrenheit
        /// </summary>
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        /// <summary>
        /// Forecast summary
        /// </summary>
        public string Summary { get; set; }
    }
}
