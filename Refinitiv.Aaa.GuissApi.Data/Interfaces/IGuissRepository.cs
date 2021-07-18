using System.Collections.Generic;
using System.Threading.Tasks;
using Refinitiv.Aaa.Pagination.Models;
using Refinitiv.Aaa.GuissApi.Data.Models;

namespace Refinitiv.Aaa.GuissApi.Data.Interfaces
{
    /// <summary>
    /// Represents a class used to access Guiss items from a datastore.
    /// </summary>
    public interface IGuissRepository
    {
        /// <summary>Returns all items from a datastore.</summary>
        /// <returns>A list of items from the datastore.</returns>
        Task<IEnumerable<GuissDb>> FindAllAsync();

        /// <summary>Returns a item with the specified ID.</summary>
        /// <param name="id">Unique ID of the item.</param>
        /// <returns>An item from the datastore.</returns>
        Task<GuissDb> FindByIdAsync(string id);

        /// <summary>
        /// Returns items from a datastore with paging.
        /// </summary>
        /// <param name="cursor">The Cursor object.</param>
        /// <returns>A list of items from the datastore with paging.</returns>
        Task<IEnumerable<GuissDb>> GetAllAsync(Cursor<GuissFilter> cursor);

        /// <summary>Saves an item to the datastore.</summary>
        /// <param name="item">Item to save.</param>
        /// <returns>Task.</returns>
        Task<GuissDb> SaveAsync(GuissDb item);

        /// <summary>
        /// Deletes an item from the datastore with the specified ID.
        /// </summary>
        /// <param name="id">Unique id of the item to delete.</param>
        /// <returns>Task.</returns>
        Task DeleteAsync(string id);
    }
}
