using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Refinitiv.Aaa.GuissApi.Interfaces.Models.Configuration
{
    /// <summary>
    /// Represents the Cache section of the configuration file.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class CacheConfig
    {
        /// <summary>
        /// Gets the expiration time in minutes for cache.
        /// </summary>
        /// <value>The expiration time in minutes for cache.</value>
        [Required]
        public int ExpirationMinutes { get; set; }
    }
}
