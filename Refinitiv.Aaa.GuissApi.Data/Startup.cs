﻿using System;
using System.Diagnostics.CodeAnalysis;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refinitiv.Aaa.GuissApi.Data.Interfaces;
using Refinitiv.Aaa.GuissApi.Data.Models;
using Refinitiv.Aaa.GuissApi.Data.Repositories;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.UserAttribute;

namespace Refinitiv.Aaa.GuissApi.Data
{
    /// <summary>
    /// Data Startup.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class Startup
    {
        /// <summary>
        /// Registers the DynamoDB dependencies. This should only be called once per API.
        /// </summary>
        /// <param name="services">IServiceCollection parameter.</param>
        /// <param name="configuration">IConfiguration parameter.</param>
        /// <returns>Returns IServiceCollection.</returns>
        public static IServiceCollection ConfigureDatabase(this IServiceCollection services,
            IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var dynamoDbConfig = new AmazonDynamoDBConfig();

            var serviceUrl = configuration["AppSettings:DynamoDb:ServiceUrl"];

            if (!string.IsNullOrWhiteSpace(serviceUrl))
            {
                dynamoDbConfig.ServiceURL = serviceUrl;
            }

            return services
                .AddSingleton(dynamoDbConfig)
                .AddScoped<IAmazonDynamoDB>(s => new AmazonDynamoDBClient(s.GetService<AmazonDynamoDBConfig>()))
                .AddScoped<IDynamoDBContext, DynamoDBContext>()
                .AddScoped<IUserAttributeRepository, UserAttributeRepository>()
                .AddScoped<IDynamoDbDocumentQueryWrapper<UserAttributeDb, UserAttributeFilter>, UserAttributeQueryWrapper>();
        }
    }
}
