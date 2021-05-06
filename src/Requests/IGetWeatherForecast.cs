using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Weathy.Models;

namespace Weathy.Requests
{
    public interface IGetWeatherForecast
    {
        /// <summary>
        /// Runs the request and returns weather forecasts for the requested city.
        /// </summary>
        /// <param name="cityName">City name</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        Task<IEnumerable<WeatherForecast>> RunAsync(string cityName, CancellationToken cancellationToken = default);
    }
}
