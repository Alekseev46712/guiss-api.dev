using System.Diagnostics.CodeAnalysis;

namespace Refinitiv.Aaa.GuissApi.Interfaces.Models.Configuration
{
    /// <summary>
    ///  Represents attributes section in attributes config file.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class UserAttributeConfig
    {
        /// <summary>
        /// Gets or sets a response path for SelectToken operation.
        /// </summary>
        /// <value>The response path for SelectToken operation.</value>
        public string ResponsePath { get; set; }
    }
}
