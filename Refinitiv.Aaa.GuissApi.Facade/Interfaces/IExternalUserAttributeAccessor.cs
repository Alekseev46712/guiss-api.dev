using System.Collections.Generic;
using System.Threading.Tasks;

namespace Refinitiv.Aaa.GuissApi.Facade.Interfaces
{
    /// <summary>
    /// Accessor that provides user attributes from external APIs.
    /// </summary>
    public interface IExternalUserAttributeAccessor : IUserAttributeAccessor
    {
        /// <summary>
        /// Gets all the user attributes related to current API.
        /// </summary>
        /// <returns>The user attributes related to current API.</returns>
        Task<IEnumerable<string>> GetDefaultAttributesAsync();
    }
}
