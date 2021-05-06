using System.Threading.Tasks;
using Weathy.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Weathy.Controllers.V2
{
    [ApiController]
    [ApiVersion("2.0")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/weather")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger _log;
        private readonly IGetWeatherForecast _getWeatherForecast;

        public WeatherForecastController(IGetWeatherForecast getWeatherForecast)
        {
            _log = Log.ForContext<WeatherForecastController>();
            _getWeatherForecast = getWeatherForecast;
        }

        /// <summary>
        /// Retrieve weather forecast for the requested city
        /// </summary>
        /// <param name="city">The name of the city</param>
        /// <returns>Collection of weather forecasts</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                _log.Warning("city parameter is missing or invalid");
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            var result = await _getWeatherForecast.RunAsync(city);
            return Ok(result);
        }
    }
}
