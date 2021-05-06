using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Weathy.Test.IntergrationTests
{
    public sealed class TestFixture<TStartup> : IDisposable
    {
        private readonly TestServer _server;
        private readonly IServiceScope _serviceScope;

        public HttpClient Client { get; }

        public TestFixture()
        {
            var appSettings = new Dictionary<string, string>
            {
                { "WEATHER_API_KEY", "925ac4872adb4b6596483701211704" },
                { "WEATHER_API_BASEURL", "https://api.weatherapi.com/" },
                { "WEATHER_API_NUMDAYS", "5" },
            };

            var hostBuilder = new WebHostBuilder()
                .UseConfiguration(new ConfigurationBuilder()
                    .AddInMemoryCollection(appSettings)
                    .Build())
                .UseStartup(typeof(TStartup))
                .UseSerilog();
            _server = new TestServer(hostBuilder);

            Client = _server.CreateClient();
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var scopeFactory = _server.Host.Services.GetService<IServiceScopeFactory>();
            _serviceScope = scopeFactory.CreateScope();
        }

        public void Dispose()
        {
            _server.Dispose();
            _serviceScope.Dispose();
        }
    }
}
