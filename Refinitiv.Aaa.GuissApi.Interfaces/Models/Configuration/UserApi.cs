using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Refinitiv.Aaa.GuissApi.Interfaces.Models.Configuration
{
    /// <summary>
    /// Represents the Services section of the configuration file.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class UserApi
    {
        /// <summary>
        /// Gets the list of the attribute names.
        /// </summary>
        /// <value>The list of the attribute names.</value>
        public IEnumerable<string> Attributes { get; set; }
    }
}
