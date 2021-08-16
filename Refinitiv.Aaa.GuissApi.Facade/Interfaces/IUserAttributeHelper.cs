using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Refinitiv.Aaa.GuissApi.Data.Models;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.UserAttribute;

namespace Refinitiv.Aaa.GuissApi.Facade.Interfaces
{
    /// <summary>
    /// Helper to perform CRUD operations on UserAttributes.
    /// </summary>
    public interface IUserAttributeHelper
    {
        /// <summary>
        /// Gets all attributes for user with the specified userUuid.
        /// </summary>
        /// <param name="userUuid">UserUuid of the item to get.</param>
        /// <returns>User attributes.</returns>
        Task<JObject> GetAllByUserUuidAsync(string userUuid);

        /// <summary>
        /// Get UserAttribute by UserUuid and Name.
        /// </summary>
        /// <param name="userUuid">UserUuid of the item to get.</param>
        /// <param name="name">UserName of the item to get.</param>
        /// <returns>User attributes.</returns>
        Task<UserAttributeDb> FindByUserUuidAndNameAsync(string userUuid, string name);

        /// <summary>Delete UserAttribute by UserUuid and Name.</summary>
        /// <param name="userUuid">UserUuid of the item to delete.</param>
        /// <param name="name">UserName of the item to delete.</param>
        /// <returns>Task.</returns>
        Task DeleteUserAttributeAsync(string userUuid, string name);

        /// <summary>
        /// Inserts User Attribute
        /// </summary>
        /// <param name="userAttributeDetails">User Attribute Details.</param>
        /// <returns>User attribute.</returns>
        Task<UserAttribute> InsertAsync(UserAttributeDetails userAttributeDetails);

        /// <summary>
        /// Updates User Attribute
        /// </summary>
        /// <param name="userAttribute">User Attribute .</param>
        /// <param name="value">Value to update .</param>
        /// <returns>User attribute.</returns>
        Task<UserAttribute> UpdateAsync(UserAttribute userAttribute, string value);

        /// <summary>
        /// Gets selected attributes for user with the specified userUuid.
        /// </summary>
        /// <param name="userUuid">UserUuid of the item to get.</param>
        /// <param name="attributes">Comma separated string of the attribute names to get.</param>
        /// <returns>User attributes.</returns>
        Task<JObject> GetAttributesByUserUuidAsync(string userUuid, string attributes);

        /// <summary>
        /// Gets selected attributes by namespaces for user with the specified userUuid.
        /// </summary>
        /// <param name="userUuid">UserUuid of the item to get.</param>
        /// <param name="namespaces">Comma separated string of the namespaces to get.</param>
        /// <returns>User attributes.</returns>
        Task<JObject> GetAttributesByUserNamespacesAndUuidAsync(string userUuid, string namespaces);

    }
}
