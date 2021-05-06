using System;
using System.Linq;
using Weathy.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Weathy.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/weather")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] s_summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger _log;

        public WeatherForecastController()
        {
            _log = Log.ForContext<WeatherForecastController>();
        }

        /// <summary>
        /// Retrieve weather forecast
        /// </summary>
        /// <returns>Collection of weather forecasts</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Get()
        {
            var rng = new Random();
            var result = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = s_summaries[rng.Next(s_summaries.Length)]
            }).ToArray();
            return Ok(result);
        }
    }
}
