﻿using System;
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
using Refinitiv.Aaa.MessageBus.Amazon;
using Refinitiv.Aaa.GuissApi.Facade.Validation;
using Refinitiv.Aaa.GuissApi.Facade.Mapping;
using Amazon.SimpleSystemsManagement;
using Refinitiv.Aaa.Ciam.SharedLibrary.Services.Interfaces;
using Refinitiv.Aaa.Ciam.SharedLibrary.Services.Services;
using Refinitiv.Aaa.Ciam.SharedLibrary.Services.Extensions;

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

            services.AddAutoMapper(
               typeof(UserAttributeMappingProfile).Assembly);

           services
                .AddScoped<IMessageHandler, MessageHandler>()
                .AddScoped<IPaginationService, PaginationService>()
                .AddScoped(typeof(ILoggerHelper<>), typeof(LoggerHelper<>))
                .AddScoped<IUserAttributeHelper, UserAttributeHelper>()
                .AddScoped<IUserAttributeValidator, UserAttributeValidator>()
                .AddScoped<IUserHelper, UserHelper>()
                .AddScoped<IUserAttributeAccessorHelper, UserAttributeAccessorHelper>()
                .AddScoped<IUserAttributeProvider, UserAttributeProvider>()
                .AddScoped<IUserAttributeConfigHelper, UserAttributeConfigHelper>()
                .AddScoped<DynamoDbUserAttributeAccessor>()
                .AddScoped<UserApiAttributeAccessor>()
                .AddSingleton<IDataCacheService, DataCacheService>()
                .ConfigureDatabase(configuration)
                .ConfigureAwsMessageBus(configuration);

            services.AddScoped<ErrorHandlingDelegatingHandler>();

            services
                .AddMemoryCacheService()
                .AddScoped<IAmazonSimpleSystemsManagement, AmazonSimpleSystemsManagementClient>((provider) => new AmazonSimpleSystemsManagementClient(new AmazonSimpleSystemsManagementConfig()))
                .AddScoped<IParameterStoreService, SimpleParameterStoreCachedService>();

            return services;
        }
    }
}
