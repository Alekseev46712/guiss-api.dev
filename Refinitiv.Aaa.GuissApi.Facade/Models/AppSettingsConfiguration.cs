using System;
using Microsoft.Extensions.Configuration;
using Refinitiv.Aaa.GuissApi.Facade.Interfaces;

namespace Refinitiv.Aaa.GuissApi.Facade.Models
{
    /// <inheritdoc />
    internal sealed class AppSettingsConfiguration : IAppSettingsConfiguration
    {
        private const int DefaultQueryLimitConst = 50;
        private readonly IConfigurationSection configSection;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppSettingsConfiguration"/> class.
        /// </summary>
        /// <param name="configuration">The Application configuration.</param>
        public AppSettingsConfiguration(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            configSection = configuration.GetSection("AppSettings");
        }

        /// <summary>
        /// Gets the Id of the application from configuration.
        /// </summary>
        /// <value>String containing the ApplicationId.</value>
        public string ApplicationId => configSection["ApplicationId"];

        /// <summary>
        /// Gets the default query limit set in configuration.
        /// </summary>
        /// <value>Int value containing the DefaultQueryLimit.</value>
        public int DefaultQueryLimit => int.TryParse(configSection["DynamoDb:DefaultQueryLimit"], out var defaultQueryLimit) ? defaultQueryLimit : DefaultQueryLimitConst;
    }
}
