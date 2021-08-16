using Refinitiv.Aaa.GuissApi.Data.Models;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.UserAttribute;
using Refinitiv.Aaa.Pagination.Models;
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
        /// Get the specified user attribute item by UserUuid and Name.
        /// </summary>
        /// <param name="userUuid">UserUuid of the item to find.</param>
        /// <param name="name">Name of the item to find.</param>
        /// <returns>A user attribute db model.</returns>
        Task<UserAttributeDb> FindByUserUuidAndNameAsync(string userUuid, string name);

        /// <summary>
        /// Search for the UserAttributes by specified filter.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<IEnumerable<UserAttributeDb>> SearchAsync(UserAttributeFilter filter);

        /// <summary>
        /// Search for the UserAttributes by specified filter including pagination.
        /// </summary>
        /// <param name="cursor">Cursor for pagination.</param>
        /// <returns>A tuple of UserAttributes, first item token, and last item token.</returns>
        Task<(IEnumerable<UserAttributeDb>, string FirstItemToken, string LastItemToken)> SearchAsync(Cursor<UserAttributeFilter> cursor);

        /// <summary>
        /// Save the UserAttribute item.
        /// </summary>
        /// <param name="item">UserAttribute item to save.</param>
        /// <returns>Saved UserAttributeDb object.</returns>
        Task<UserAttributeDb> SaveAsync(UserAttributeDb item);

        /// <summary>s
        /// Deletes UserAttribute from the database.
        /// </summary>
        /// <param name="userUuid">UserUuid item to delete.</param>
        /// <param name="name">UserName item to delete.</param>
        Task DeleteAsync(string userUuid, string name);
    }
}
