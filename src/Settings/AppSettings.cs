/*
 * F-Secure Cloud Protection
 *
 * Copyright (c) 2021 F-Secure Corporation
 * All rights reserved
 */

namespace Weathy.Settings
{
    /// <summary>
    /// Abstracts application settings passed via appsettings.json,
    /// command line or environment variables.
    /// </summary>
    public class AppSettings
    {
        public bool ENABLE_HTTP_LOGGING { get; set; } = false;
    }
}
