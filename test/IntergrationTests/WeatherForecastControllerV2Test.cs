using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Weathy.Models;
using Xunit;

namespace Weathy.Test.IntergrationTests
{
    [Collection("Sequential")]
    public class WeatherForecastControllerV2Test : IClassFixture<TestFixture<Startup>>
    {
        private readonly TestFixture<Startup> _fixture;
        private readonly HttpClient _client;
        private const string RequestUri = "/api/v2/weather";

        public WeatherForecastControllerV2Test(TestFixture<Startup> fixture)
        {
            _fixture = fixture;
            _client = fixture.Client;
        }

        [Fact]
        public async Task GetWeatherForecastShouldReturnOk()
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, RequestUri + "?city=London")
            {
                Content = new StringContent(string.Empty, Encoding.UTF8, "application/json")
            };

            using var response = await _client.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<WeatherForecast>>(jsonString);

            Assert.True(result.Count > 0);
        }

        [Fact]
        public async Task GetWeatherForecastShouldReturnBadRequest()
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, RequestUri)
            {
                Content = new StringContent(string.Empty, Encoding.UTF8, "application/json")
            };

            using var response = await _client.SendAsync(httpRequest);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

    }
}
