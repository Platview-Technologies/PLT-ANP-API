﻿using Contracts;
using LoggerService;

namespace PLT_ANP_API.Extentions
{
    public static class ServiceExtentions
    {
        /// <summary>
        /// Configures Cross-Origin Resource Sharing (CORS) policy.
        /// </summary>
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });
        }

        /// <summary>
        /// Configures integration with Internet Information Services (IIS).
        /// </summary>
        public static void ConfigureIISIntegration(this IServiceCollection services)
        {
            services.Configure<IISOptions>(options =>
            {
                // Configure IIS options here, if needed
            });
        }

        /// <summary>
        /// Registers the logger service for logging application events.
        /// </summary>
        public static void ConfigureLoggerService(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerManager, LoggerManager>();
        }

    }
}
