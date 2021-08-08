using System.Diagnostics.CodeAnalysis;

namespace Refinitiv.Aaa.GuissApi.Interfaces.Models.Configuration
{
    /// <summary>
    ///  Represents properties from appsettings file for services configuration.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Services
    {
        /// <summary>
        /// Gets UserApi configuration.
        /// </summary>
        /// <value>UserApi configuration object.</value>
        public UserApi UserApi { get; set; }
    }
}
