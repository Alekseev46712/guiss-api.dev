using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Refinitiv.Aaa.GuissApi.Interfaces.Models.Configuration
{
    /// <summary>
    /// Represents the DynamoDb section of the configuration file.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class DynamoDbConfig
    {
        /// <summary>
        /// Gets the name of the DynamoDB table for UserAttributeTableName.
        /// </summary>
        /// <value>The name of the DynamoDB table for UserAttributeTableName.</value>
        [Required]
        public string UserAttributeTableName { get; set; }
    }
}
