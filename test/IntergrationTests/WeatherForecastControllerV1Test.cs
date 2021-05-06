using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Weathy.Models;
using Xunit;

namespace Weathy.Test.IntergrationTests
{
    [Collection("Sequential")]
    public class WeatherForecastControllerV1Test : IClassFixture<TestFixture<Startup>>
    {
        private readonly TestFixture<Startup> _fixture;
        private readonly HttpClient _client;
        private const string RequestUri = "/api/v1/weather";

        public WeatherForecastControllerV1Test(TestFixture<Startup> fixture)
        {
            _fixture = fixture;
            _client = fixture.Client;
        }

        [Fact]
        public async Task GetWeatherForecastShouldReturnOk()
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, RequestUri)
            {
                Content = new StringContent(string.Empty, Encoding.UTF8, "application/json")
            };

            using var response = await _client.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<WeatherForecast>>(jsonString);

            Assert.True(result.Count > 0);
        }
    }
}
