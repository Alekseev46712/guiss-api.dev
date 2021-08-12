using System.Diagnostics.CodeAnalysis;

namespace Refinitiv.Aaa.GuissApi.Interfaces.Models.Configuration
{
    /// <summary>
    ///  Represents properties from appsettings file.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class AppSettingsConfig
    {
        /// <summary>
        /// Gets DynamoDb configuration.
        /// </summary>
        /// <value>DynamoDb configuration object.</value>
        public DynamoDbConfig DynamoDb { get; set; }

        public CacheConfig Cache { get; set; }
    }
}
