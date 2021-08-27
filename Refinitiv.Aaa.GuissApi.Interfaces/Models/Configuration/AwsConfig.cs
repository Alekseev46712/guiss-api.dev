using System.Diagnostics.CodeAnalysis;

namespace Refinitiv.Aaa.GuissApi.Interfaces.Models.Configuration
{
    /// <summary>
    /// Represents the Aws section of the configuration file.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class AwsConfig
    {
        /// <summary>
        /// Gets parameter store path.
        /// </summary>
        /// <value>The parameter store path.</value>
        public string ParameterStorePath { get; set; }
    }
}
