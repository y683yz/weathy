/*
 * F-Secure Cloud Protection
 *
 * Copyright (c) 2021 F-Secure Corporation
 * All rights reserved
 */

using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using Weathy.HealthChecks;
using Weathy.Requests;
using Weathy.Settings;
using Weathy.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Weathy
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppSettings>(_configuration);

            services.AddControllers(options =>
            {
                options.EnableEndpointRouting = false;
            });
            services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
            });

            // Enable API versioning
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
            });
            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            // Register Swagger generator if enabled
            if (_configuration.GetValue<bool>("ENABLE_SWAGGER_UI"))
            {
                services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
                services.AddSwaggerGen(options =>
                {
                    // add a custom operation filter which sets default values
                    options.OperationFilter<SwaggerDefaultValues>();

                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    options.IncludeXmlComments(xmlPath);
                });
            }

            // Enable HTTP logging if needed
            if (_configuration.GetValue<bool>("ENABLE_HTTP_LOGGING"))
            {
                services.AddLogging(loggingBuilder =>
                {
                    loggingBuilder.AddFilter((category, level) => true);
                    // Add console logger so we can see all the logging produced by the client by default.
                    loggingBuilder.AddSimpleConsole(console => console.IncludeScopes = true);
                });
            }

            // Enable HTTP Client to communicate with other services
            services.AddHttpClient(Options.DefaultName)
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    var httpClientHandler = new HttpClientHandler();

                    if (_configuration.GetValue<bool>("IGNORE_TLS_ERRORS"))
                    {
                        httpClientHandler.ServerCertificateCustomValidationCallback =
                            (message, cert, chain, errors) => true;
                        Log.Warning("Using insecure TLS connections");
                    }

                    return httpClientHandler;
                })
                .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(new[]
                {
                    // Creates a policy which will handle typical transient faults, retrying
                    // the underlying http request up to 3 times if necessary. The policy
                    // will apply a delay of 1 second before the first retry; 5 seconds before
                    // a second retry; and 10 seconds before the third.
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10)
                }));

            // Enable Weather API to fetch weather forecast from the external service
            services.AddTransient<IGetWeatherForecast, GetWeatherForecast>()
                .AddSingleton<WeatherApiSettings>(_ => new WeatherApiSettings()
                {
                    ApiKey = _configuration.GetValue<String>("WEATHER_API_KEY"),
                    BaseUrl = _configuration.GetValue<String>("WEATHER_API_BASEURL"),
                    NumDays = _configuration.GetValue<int>("WEATHER_API_NUMDAYS"),
                });

            // Enable the health check service
            services.AddHealthChecks().AddCheck<ServiceHealthCheck>("ServiceHealthCheck");
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="provider"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            if (_configuration.GetValue<bool>("ENABLE_SWAGGER_UI"))
            {
                app.UseSwagger(options =>
                {
                    options.RouteTemplate = "docs/{documentName}/docs.json";
                });
                app.UseSwaggerUI(options =>
                {
                    options.RoutePrefix = "docs";
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"/docs/{description.GroupName}/docs.json",
                            description.GroupName.ToUpperInvariant());
                    }
                });
            }

            app.UseHealthChecks("/health");
            app.UseRouting();
            app.UseAuthorization();
            app.UseSerilogRequestLogging();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
