using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.Extensions.Logging;
using Refinitiv.Aaa.Interfaces.Business;
using Refinitiv.Aaa.Pagination.Models;

namespace Refinitiv.Aaa.GuissApi.Data.Repositories
{
    /// <summary>
    /// A wrapper for the DynamoDB Document API.
    /// </summary>
    /// <typeparam name="T">DynamoDB object.</typeparam>
    /// <typeparam name="TFilter">IFilter object.</typeparam>
    [ExcludeFromCodeCoverage]
    public class BaseQueryWrapper<T, TFilter>
        where TFilter : IFilter
    {
        private readonly IAmazonDynamoDB amazonDynamoDb;
        private readonly ILogger<BaseQueryWrapper<T, TFilter>> logger;
        private readonly IDynamoDBContext dynamoDb;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseQueryWrapper{T, TFilter}"/> class.
        /// </summary>
        /// <param name="amazonDynamoDb">Instance of IAmazonDynamoDB.</param>
        /// <param name="dynamoDb">Dynamodb instance.</param>
        /// <param name="logger">Logger to write errors to.</param>
        protected BaseQueryWrapper(IAmazonDynamoDB amazonDynamoDb, IDynamoDBContext dynamoDb, ILogger<BaseQueryWrapper<T, TFilter>> logger)
        {
            this.amazonDynamoDb = amazonDynamoDb;
            this.dynamoDb = dynamoDb;
            this.logger = logger;
        }

        /// <param name="tableName">Table name.</param>
        /// <param name="cursor">Cursor with filter.</param>
        /// <param name="queryOperationConfig">Query operation config.</param>
        public Task<IEnumerable<T>> PerformQueryAsync(string tableName, Cursor<TFilter> cursor, QueryOperationConfig queryOperationConfig)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            if (cursor == null)
            {
                throw new ArgumentNullException(nameof(cursor));
            }

            return InternalPerformQueryAsync();

            async Task<IEnumerable<T>> InternalPerformQueryAsync()
            {
                try
                {
                    var table = Table.LoadTable(amazonDynamoDb, tableName);
                    var query = table.Query(queryOperationConfig);
                    var response = await query.GetNextSetAsync();

                    while (!query.IsDone && response.Count < queryOperationConfig.Limit)
                    {
                        response.AddRange(await query.GetNextSetAsync());
                    }

                    if (cursor.BackwardSearch)
                    {
                        response.Reverse();
                    }

                    cursor.LastEvaluatedKey = query.PaginationToken;

                    return response.Select(r => dynamoDb.FromDocument<T>(r));
                }
                catch (AmazonDynamoDBException ex)
                {
                    logger.LogError(ex, $"{nameof(PerformQueryAsync)}: " +
                                        $"An exception has occurred while perform query.");
                    throw;
                }
            }
        }
    }
}
