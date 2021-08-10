using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Refinitiv.Aaa.GuissApi.Interfaces.Models.Configuration
{
    /// <summary>
    ///  Represents attributes config file.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class UserAttributeApiConfig
    {
        /// <summary>
        /// Gets or sets a name of the external api.
        /// </summary>
        /// <value>Name of the external API.</value>
        public string ApiName { get; set; }

        /// <summary>
        /// Gets or sets an URL that gets the attributes.
        /// </summary>
        /// <value>URL that gets the attributes.</value>
        public string UrlTemplate { get; set; }

        /// <summary>
        /// Gets or sets an attributes config object.
        /// </summary>
        /// <value>Attributes config object.</value>
        public Dictionary<string, UserAttributeConfig> Attributes { get; set; }
    }
}
