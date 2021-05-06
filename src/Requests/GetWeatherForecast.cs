using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Weathy.Models;
using Weathy.Settings;
using Serilog;

namespace Weathy.Requests
{
    public class GetWeatherForecast : IGetWeatherForecast
    {
        private readonly ILogger _log;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly WeatherApiSettings _settings;

        public GetWeatherForecast(IHttpClientFactory httpClientFactory, WeatherApiSettings settings)
        {
            _log = Log.ForContext<GetWeatherForecast>();
            _httpClientFactory = httpClientFactory;
            _settings = settings;
        }

        public async Task<IEnumerable<WeatherForecast>> RunAsync(string cityName, CancellationToken cancellationToken = default)
        {
            _log.Debug("Enter {Class}.{Method}", nameof(GetWeatherForecast), nameof(GetWeatherForecast.RunAsync));
            try
            {
                // Create a request to Weather API
                var query = HttpUtility.ParseQueryString(string.Empty);
                query["key"] = _settings.ApiKey;
                query["q"] = cityName;
                query["days"] = _settings.NumDays.ToString();
                var requestUri = _settings.BaseUrl + "v1/forecast.json?" + query.ToString();
                var request = new HttpRequestMessage(HttpMethod.Get, requestUri)
                {
                    Content = new StringContent(string.Empty, Encoding.UTF8, "application/json"),
                };
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Call Weather API endpoint
                _log.Debug("Calling {RequestUri}...", requestUri);
                using var response = await _httpClientFactory.CreateClient().SendAsync(request, cancellationToken);

                // Handle the response
                var responseContent = await response.Content.ReadAsStringAsync();
                dynamic result = JsonSerializer.Deserialize<ExpandoObject>(responseContent);
                var days = result.forecast.GetProperty("forecastday").EnumerateArray();

                var weatherForecasts = new List<WeatherForecast>();
                foreach (var day in days)
                {
                    weatherForecasts.Add(new WeatherForecast()
                    {
                        Date = day.GetProperty("date").GetDateTime(),
                        TemperatureC = (int)day.GetProperty("day").GetProperty("avgtemp_c").GetDouble(),
                        Summary = day.GetProperty("day").GetProperty("condition").GetProperty("text").GetString()
                    });
                }

                _log.Debug("Exit {Class}.{Method}", nameof(GetWeatherForecast), nameof(GetWeatherForecast.RunAsync));
                return weatherForecasts;
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Exception getting weather forecast for city: {CityName}", cityName);
                return null;
            }
        }
    }
}
