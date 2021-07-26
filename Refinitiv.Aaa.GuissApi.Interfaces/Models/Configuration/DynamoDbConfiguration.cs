using System.ComponentModel.DataAnnotations;

namespace Refinitiv.Aaa.GuissApi.Interfaces.Models.Configuration
{
    /// <summary>
    /// Represents the AppSettings section of the configuration file.
    /// </summary>
    public sealed class DynamoDbConfiguration
    {
        /// <summary>
        /// Gets the name of the DynamoDB table for UserAttributeTableName.
        /// </summary>
        /// <value>The name of the DynamoDB table for UserAttributeTableName.</value>
        [Required]
        public string UserAttributeTableName { get; set; }
    }
}
