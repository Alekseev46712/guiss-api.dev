using Refinitiv.Aaa.GuissApi.Data.Models;
using Refinitiv.Aaa.GuissApi.Interfaces.UserAttribute;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Refinitiv.Aaa.GuissApi.Data.Interfaces
{
    /// <summary>
    /// Represents a class used to access UserAttribute items from a datastore.
    /// </summary>
    public interface IUserAttributeRepository
    {
        /// <summary>
        /// Search for the UserAttributes by specified filter.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<IEnumerable<UserAttributeDb>> SearchAsync(UserAttributeFilter filter);

        /// <summary>
        /// Save the UserAttribute item.
        /// </summary>
        /// <param name="item">UserAttribute item to save.</param>
        /// <returns>Saved UserAttributeDb object.</returns>
        Task<UserAttributeDb> SaveAsync(UserAttributeDb item);

        /// <summary>
        /// Deletes UserAttribute from the database.
        /// </summary>
        /// <param name="userUuid">UserAttribute item to delete.</param>
        Task DeleteAsync(string userUuid);
    }
}
