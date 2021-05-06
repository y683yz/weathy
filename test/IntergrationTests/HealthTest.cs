using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Weathy.Test.IntergrationTests
{
    public class HealthTest : IClassFixture<TestFixture<Startup>>
    {
        private readonly HttpClient _client;

        public HealthTest(TestFixture<Startup> fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async Task HealthCheckShoudReturnHeathly()
        {
            using (var response = await _client.GetAsync("/health"))
            {
                Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            }
        }
    }
}
