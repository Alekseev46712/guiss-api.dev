using System.Threading.Tasks;
using Refinitiv.Aaa.Interfaces.Business;
using Refinitiv.Aaa.GuissApi.Data.Models;
using Refinitiv.Aaa.GuissApi.Facade.Models;

namespace Refinitiv.Aaa.GuissApi.Facade.Interfaces
{
    /// <summary>
    /// Interface for IGuissHelper.
    /// </summary>
    public interface IGuissHelper
    {
        /// <summary>Gets a enumerable of all items.</summary>
        /// <param name="filter">Filter to be used on the result set.</param>
        /// <param name="paging">Paging to be used on the result set.</param>
        /// <returns>A result set.</returns>
        Task<IResultSet<Guiss>> FindAllAsync(GuissFilter filter, IPagingModel paging);

        /// <summary>Gets an item with the specified ID.</summary>
        /// <param name="id">ID of the item to get.</param>
        /// <returns>The item requested.</returns>
        Task<Guiss> FindByIdAsync(string id);

        /// <summary>Inserts a new item.</summary>
        /// <param name="item">item to insert.</param>
        /// <returns>Task with the item inserted.</returns>
        Task<Guiss> InsertAsync(Guiss item);

        /// <summary>Updates an item with the specified ID.</summary>
        /// <param name="item">item to update.</param>
        /// <returns>Task with the updated item.</returns>
        Task<Guiss> UpdateAsync(Guiss item);

        /// <summary>Delete the item with the specified ID.</summary>
        /// <param name="id">ID of the item to delete.</param>
        /// <returns>Task.</returns>
        Task DeleteAsync(string id);
    }
}
