using System.Collections.Generic;
using System.Threading.Tasks;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.UserAttribute;

namespace Refinitiv.Aaa.GuissApi.Facade.Interfaces
{
    /// <summary>
    /// Provides user attributes.
    /// </summary>
    public interface IUserAttributeAccessor
    {
        /// <summary>
        /// Provides user attributes by user uuid and attribute names.
        /// </summary>
        /// <param name="userUuid">The user uuid.</param>
        /// <param name="attributeNames">The attribute names.</param>
        Task<IEnumerable<UserAttributeDetails>> GetUserAttributesAsync(string userUuid, IEnumerable<string> attributeNames);
    }
}
