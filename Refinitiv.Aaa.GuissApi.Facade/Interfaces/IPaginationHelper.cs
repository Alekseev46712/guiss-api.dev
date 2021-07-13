using Refinitiv.Aaa.Interfaces.Business;
using Refinitiv.Aaa.Pagination.Models;
using Refinitiv.Aaa.GuissApi.Data.Models;

namespace Refinitiv.Aaa.GuissApi.Facade.Interfaces
{
    /// <summary>
    /// Pagination Helper.
    /// </summary>
    public interface IPaginationHelper
    {
        /// <summary>
        /// Method to initially setup Cursor.
        /// </summary>
        /// <param name="filterValue">The filterValue.</param>
        /// <param name="resultSet">Instance of IResultSet.</param>
        /// <param name="paging">Instance of IPagingModel.</param>
        /// <typeparam name="TResultSet">The ResultSet type parameter.</typeparam>
        /// <returns>A Cursor object.</returns>
        Cursor<GuissFilter> SetupCursor<TResultSet>(GuissFilter filterValue, IResultSet<TResultSet> resultSet, IPagingModel paging = null)
            where TResultSet : IModel;

        /// <summary>
        /// Method to create a pagination token.
        /// </summary>
        /// <param name="cursor">The Cursor object.</param>
        /// <returns>Pagination token string.</returns>
        string CreatePaginationToken(Cursor<GuissFilter> cursor);
    }
}
