using System.ComponentModel.DataAnnotations;

namespace Refinitiv.Aaa.GuissApi.Interfaces.Configuration
{
    /// <summary>
    /// Represents the AppSettings section of the configuration file.
    /// </summary>
    public sealed class DynamoDbConfiguration
    {
        /// <summary>
        /// Gets the name of the UserAttribute DynamoDB table.
        /// </summary>
        /// <value>The name of the UserAttribute DynamoDB table.</value>
        [Required]
        public string UserAttributeTableName { get; set; }
    }
}
