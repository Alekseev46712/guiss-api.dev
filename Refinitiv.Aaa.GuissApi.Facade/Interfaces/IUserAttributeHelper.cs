using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
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
        Task<UserAttribute> InsertAsync(UserAttribute item);
    }
}
