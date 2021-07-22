using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refinitiv.Aaa.Foundation.ApiClient.Helpers;
using Refinitiv.Aaa.Foundation.ApiClient.Interfaces;
using Refinitiv.Aaa.Pagination;
using Refinitiv.Aaa.Pagination.Interfaces;
using Refinitiv.Aaa.GuissApi.Data;
using Refinitiv.Aaa.GuissApi.Facade.Helpers;
using Refinitiv.Aaa.GuissApi.Facade.Interfaces;
using Refinitiv.Aaa.GuissApi.Facade.Models;
using Refinitiv.Aaa.MessageBus.Amazon;

namespace Refinitiv.Aaa.GuissApi.Facade
{
    /// <summary>
    /// Registers services with the dependency injection framework.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class Startup
    {
        /// <summary>
        /// Registers the helper services with the dependency injection framework.
        /// </summary>
        /// <param name="services">Service collection to be updated.</param>
        /// <param name="configuration">Configuration settings.</param>
        /// <returns>Service collection, to allow method chaining.</returns>
        public static IServiceCollection ConfigureFacade(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            services
                .AddSingleton<IAppSettingsConfiguration, AppSettingsConfiguration>()
                .AddScoped<IGuissHelper, GuissHelper>()
                .AddScoped<IMessageHandler, MessageHandler>()
                .AddScoped<IPaginationService, PaginationService>()
                .AddScoped<IPaginationHelper, PaginationHelper>()
                .AddScoped(typeof(ILoggerHelper<>), typeof(LoggerHelper<>))
                .AddScoped<IUserAttributeHelper, UserAttributeHelper>()
                .ConfigureDatabase(configuration)
                .ConfigureDataDependencies(configuration)
                .ConfigureAwsMessageBus(configuration);

            return services;
        }
    }
}
