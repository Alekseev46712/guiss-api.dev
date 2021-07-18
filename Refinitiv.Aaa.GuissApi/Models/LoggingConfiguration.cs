using System.Diagnostics.CodeAnalysis;

namespace Refinitiv.Aaa.GuissApi.Models
{
    /// <summary>
    /// Logging Configuration class bound from appsettings file
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class LoggingConfiguration
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="LoggingConfiguration"/> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        public string Context { get; set; }

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        /// <value>
        /// The category.
        /// </value>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the account pool identifier.
        /// </summary>
        /// <value>
        /// The account pool identifier.
        /// </value>
        public string IdentityPoolId { get; set; }

        /// <summary>
        /// Gets or sets the target.
        /// </summary>
        /// <value>
        /// The target.
        /// </value>
        public string Target { get; set; }

        /// <summary>
        /// Gets or sets the endpoint.
        /// </summary>
        /// <value>
        /// The endpoint.
        /// </value>
        public string Endpoint { get; set; }
    }
}
