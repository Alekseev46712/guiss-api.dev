using System.Collections.Generic;
using System.Threading.Tasks;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.UserAttribute;

namespace Refinitiv.Aaa.GuissApi.Facade.Interfaces
{
    /// <summary>
    /// Provides user attributes.
    /// </summary>
    public interface IUserAttributeProvider
    {
        /// <summary>
        /// Provides all the user attributes by user uuid.
        /// </summary>
        /// <param name="userUuid"></param>
        Task<IEnumerable<UserAttributeDetails>> GetUserAttributesAsync(string userUuid);

        /// <summary>
        /// Provides specified user attributes by user uuid.
        /// </summary>
        /// <param name="userUuid"></param>
        /// <param name="attributeNames"></param>
        Task<IEnumerable<UserAttributeDetails>> GetUserAttributesAsync(string userUuid, IEnumerable<string> attributeNames);
    }
}
