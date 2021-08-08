using System.Collections.Generic;

namespace Refinitiv.Aaa.GuissApi.Facade.Interfaces
{
    /// <summary>
    /// Accessor that provides user attributes from external APIs.
    /// </summary>
    public interface IExternalUserAttributeAccessor : IUserAttributeAccessor
    {
        /// <summary>
        /// All the user attributes related to current API.
        /// </summary>
        IEnumerable<string> DefaultAttributes { get; }
    }
}
