/*
 * F-Secure Cloud Protection
 *
 * Copyright (c) 2021 F-Secure Corporation
 * All rights reserved
 */

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Weathy
{
    public static class Program
    {
        /// <summary>
        /// The main entry point of the application
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task Main(string[] args)
        {
            try
            {
                var webHost = BuildWebHost(args);

                Log.Information("*** Starting the service... ***");
                Log.Information("Using framework: {Location}", Path.GetDirectoryName(typeof(object).Assembly.Location));

                await webHost.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "The service terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        /// <summary>
        /// Build the web hoster that will run the service
        /// </summary>
        /// <param name="args"></param>
        /// <returns>IWebHost</returns>
        public static IWebHost BuildWebHost(string[] args)
        {
            // Create web host that will run the service
            var webHost = new WebHostBuilder()
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.SetBasePath(Directory.GetCurrentDirectory());

                    // Get app settings in the below order, every following configuration
                    // overrides the previous.
                    configApp.AddJsonFile("appsettings.json", optional: true);
                    configApp.AddJsonFile(
                        $"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json",
                        optional: true);

                    // Add only the environment variables with prefix "WEATHY_" 
                    // to avoid overloading the space with unneeded parameters.
                    // The prefix is removed from the environment variables.
                    configApp.AddEnvironmentVariables(prefix: "WEATHY_");
                    configApp.AddCommandLine(args);
                })
                .ConfigureLogging((hostContext, configLogging) =>
                {
                    // Serilog gets all its configuration from the host configuration.
                    // Usually it is in the appsettings file.
                    Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(hostContext.Configuration)
                        .CreateLogger();
                        configLogging.AddSerilog(dispose: true);
                })
                .UseKestrel()
                .UseSerilog()
                // Configure additional services in Startup class
                .UseStartup<Startup>()
                .Build();

            return webHost;
        }
    }
}
