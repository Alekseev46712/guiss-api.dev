using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.Extensions.Logging;
using Refinitiv.Aaa.GuissApi.Data.Interfaces;
using Refinitiv.Aaa.GuissApi.Data.Models;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.UserAttribute;
using Refinitiv.Aaa.Pagination.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Refinitiv.Aaa.GuissApi.Data.Repositories
{
    /// <summary>
    /// A wrapper for the DynamoDB Document API.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class UserAttributeQueryWrapper : IDynamoDbDocumentQueryWrapper<UserAttributeDb, UserAttributeFilter>
    {
        private readonly IAmazonDynamoDB amazonDynamoDb;
        private readonly ILogger<UserAttributeQueryWrapper> logger;
        private readonly IDynamoDBContext dynamoDb;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAttributeQueryWrapper"/> class.
        /// </summary>
        /// <param name="amazonDynamoDb">Instance of IAmazonDynamoDB.</param>
        /// <param name="dynamoDb">Dynamodb instance.</param>
        /// <param name="logger">Logger to write errors to.</param>
        public UserAttributeQueryWrapper(IAmazonDynamoDB amazonDynamoDb, IDynamoDBContext dynamoDb, ILogger<UserAttributeQueryWrapper> logger)
        {
            this.amazonDynamoDb = amazonDynamoDb;
            this.dynamoDb = dynamoDb;
            this.logger = logger;
        }

        /// <summary>
        /// Queries the DynamoDb table using the Document API.
        /// </summary>
        /// <param name="tableName">Table name.</param>
        /// <param name="cursor">Cursor with filter.</param>
        /// <param name="queryOperationConfig">Query operation config.</param>
        public Task<(IEnumerable<UserAttributeDb> Items, string FirstItemToken, string LastItemToken)> PerformQueryAsync(string tableName, Cursor<UserAttributeFilter> cursor, QueryOperationConfig queryOperationConfig)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            if (cursor == null)
            {
                throw new ArgumentNullException(nameof(cursor));
            }

            return InternalPerformQueryAsync(tableName, cursor, queryOperationConfig);
        }

        private async Task<(IEnumerable<UserAttributeDb>, string FirstItemToken, string LastItemToken)> InternalPerformQueryAsync(string tableName, Cursor<UserAttributeFilter> cursor, QueryOperationConfig queryOperationConfig)
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

                var items = response.Select(r => dynamoDb.FromDocument<UserAttributeDb>(r));
                var firstItemToken = cursor.BackwardSearch ? CreateToken(items.LastOrDefault()) : CreateToken(items.FirstOrDefault());
                var lastItemToken = query.PaginationToken;

                if (!query.IsDone)
                {
                    response = await query.GetNextSetAsync();
                    if (response.Count == 0)
                    {
                        lastItemToken = query.PaginationToken;
                    }
                }

                return (items, firstItemToken, lastItemToken);
            }
            catch (AmazonDynamoDBException ex)
            {
                logger.LogError(ex, "{Method}: An exception has occurred while get AppIdentities by filter.", nameof(PerformQueryAsync));
                throw;
            }
        }

        private string CreateToken(UserAttributeDb item)
        {
            if (item == null)
            {
                return "{}";
            }

            return $"{{\"UserUuid\":{{\"S\":\"{item.UserUuid}\"}},\"Name\":{{\"S\":\"{item.Name}\"}}}}";
        }
    }
}
