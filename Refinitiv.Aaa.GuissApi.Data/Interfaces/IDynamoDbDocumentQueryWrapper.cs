using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DocumentModel;
using Refinitiv.Aaa.Interfaces.Business;
using Refinitiv.Aaa.Pagination.Models;

namespace Refinitiv.Aaa.GuissApi.Data.Interfaces
{
    /// <summary>
    /// A wrapper for the DynamoDB Document API.
    /// </summary>
    /// <typeparam name="T">DynamoDB object.</typeparam>
    /// <typeparam name="TFilter">IFilter object.</typeparam>
    public interface IDynamoDbDocumentQueryWrapper<T, TFilter>
        where TFilter: IFilter
    {
        /// <param name="tableName">Table name.</param>
        /// <param name="cursor">Cursor with filter.</param>
        /// <param name="queryOperationConfig">Query operation config.</param>
        Task<IEnumerable<T>> PerformQueryAsync(string tableName, Cursor<TFilter> cursor, QueryOperationConfig queryOperationConfig);
    }
}
