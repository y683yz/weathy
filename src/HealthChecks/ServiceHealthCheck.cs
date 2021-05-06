/*
 * F-Secure Cloud Protection
 *
 * Copyright (c) 2021 F-Secure Corporation
 * All rights reserved
 */

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Weathy.HealthChecks
{
    /// <summary>
    /// Check and provide health status of the service
    /// </summary>
    public class ServiceHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, 
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(HealthCheckResult.Healthy("OK"));
        }
    }
}
