using System.Diagnostics.CodeAnalysis;

namespace Refinitiv.Aaa.GuissApi.Interfaces.Models.Configuration
{
    /// <summary>
    /// Represents the ParameterStore section of the configuration file.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ParameterStoreConfig
    {
        /// <summary>
        /// Gets the user attribute api config parameter store name.
        /// </summary>
        /// <value>The user attribute api config parameter store name.</value>
        public string UserAttributeApiConfigParameterStoreName { get; set; }
    }
}
