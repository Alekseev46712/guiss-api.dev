using System.Diagnostics.CodeAnalysis;

namespace Refinitiv.Aaa.GuissApi.Interfaces.Models.Configuration
{
    /// <summary>
    ///  Represents properties from appsettings file for database configuration.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class AppSettings
    {
        /// <summary>
        /// Gets DynamoDb configuration.
        /// </summary>
        /// <value>DynamoDb configuration object.</value>
        public DynamoDbConfiguration DynamoDb { get; set; }

        /// <summary>
        /// Gets Services configuration.
        /// </summary>
        /// <value>Services configuration object.</value>
        public Services Services { get; set; }
    }
}
