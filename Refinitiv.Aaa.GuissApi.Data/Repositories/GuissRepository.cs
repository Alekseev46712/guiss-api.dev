using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Logging;
using Refinitiv.Aaa.Pagination.Models;
using Refinitiv.Aaa.GuissApi.Data.Constants;
using Refinitiv.Aaa.GuissApi.Data.Exceptions;
using Refinitiv.Aaa.GuissApi.Data.Interfaces;
using Refinitiv.Aaa.GuissApi.Data.Models;
using GuissFilter = Refinitiv.Aaa.GuissApi.Data.Models.GuissFilter;

namespace Refinitiv.Aaa.GuissApi.Data.Repositories
{
    /// <summary>
    /// Guiss Repository.
    /// </summary>
    public class GuissRepository : IGuissRepository
    {
        private readonly IDynamoDBContext dynamoDb;
        private readonly string tableName;
        private readonly ILogger<GuissRepository> logger;
        private readonly IAmazonDynamoDB amazonDynamoDb;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuissRepository"/> class.
        /// </summary>
        /// <param name="dynamoDb">Dynamodb instance.</param>
        /// <param name="tableName">Dynamo Table name.</param>
        /// <param name="logger">Logger to write errors to.</param>
        /// <param name="amazonDynamoDb">Instance of IAmazonDynamoDB.</param>
        public GuissRepository(IDynamoDBContext dynamoDb, string tableName, ILogger<GuissRepository> logger, IAmazonDynamoDB amazonDynamoDb)
        {
            this.dynamoDb = dynamoDb;
            this.tableName = tableName;
            this.logger = logger;
            this.amazonDynamoDb = amazonDynamoDb;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<GuissDb>> FindAllAsync()
        {
            var query = dynamoDb.QueryAsync<GuissDb>(
                ItemKinds.Guiss,
                new DynamoDBOperationConfig
                {
                    IndexName = IndexNames.KindIndex,
                    OverrideTableName = tableName,
                });

            return await query.GetRemainingAsync().ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<GuissDb> FindByIdAsync(string id)
        {
            var items = await GetGuissByIdAsync(id).ConfigureAwait(false);
            return items.SingleOrDefault();
        }

        /// <inheritdoc />
        public Task<IEnumerable<GuissDb>> GetAllAsync(Cursor<GuissFilter> cursor)
        {
            if (cursor == null)
            {
                throw new ArgumentNullException(nameof(cursor));
            }

            return InternalGetAllAsync(cursor);
        }

        /// <inheritdoc/>
        public Task<GuissDb> SaveAsync(GuissDb item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return SaveInternalAsync(item);
        }

        /// <inheritdoc/>
        public async Task DeleteAsync(string id)
        {
            var batch = dynamoDb.CreateBatchWrite<GuissDb>(
                new DynamoDBOperationConfig
                {
                    SkipVersionCheck = true,
                    OverrideTableName = tableName,
                });

            var item = await FindByIdAsync(id).ConfigureAwait(false);
            if (item == null)
            {
                logger.LogWarning($"Get template by id: {id} returned null");
                return;
            }

            batch.AddDeleteItem(item);
            await batch.ExecuteAsync().ConfigureAwait(false);
        }

        private async Task<GuissDb> SaveInternalAsync(GuissDb item)
        {
            var config = new DynamoDBOperationConfig
            {
                IgnoreNullValues = true,
                OverrideTableName = tableName,
            };

            try
            {
                await dynamoDb.SaveAsync(item, config).ConfigureAwait(false);
            }
            catch (ConditionalCheckFailedException e)
            {
                logger.LogError(e, $"Version conflict when updating item {item.Id}");
                throw new UpdateConflictException(e);
            }

            // DynamoDB automatically increments the version number, so we have to reload
            // the template from the database.
            var template = await FindByIdAsync(item.Id).ConfigureAwait(false);
            if (template == null)
            {
                logger.LogWarning($"Get template by id: {item.Id} returned null");
                return null;
            }

            return template;
        }

        private async Task<List<GuissDb>> GetGuissByIdAsync(string id)
        {
            var query = dynamoDb.QueryAsync<GuissDb>(
                $"{id}",
                QueryOperator.Equal,
                new[] { ItemKinds.Guiss },
                new DynamoDBOperationConfig { OverrideTableName = tableName });

            var items = await query.GetRemainingAsync().ConfigureAwait(false);
            return items.ToList();
        }

        private async Task<IEnumerable<GuissDb>> InternalGetAllAsync(Cursor<GuissFilter> cursor)
        {
            var queryOperationConfig = new QueryOperationConfig
            {
                Limit = cursor.Limit,
                BackwardSearch = cursor.BackwardSearch,
                PaginationToken = cursor.LastEvaluatedKey ?? "{}",
                IndexName = IndexNames.KindIndex,
                Filter = new QueryFilter("Kind", QueryOperator.Equal, ItemKinds.Guiss),
            };

            var items = await PerformPaginatedQueryAsync(cursor, queryOperationConfig).ConfigureAwait(false);
            return items;
        }

        private async Task<IEnumerable<GuissDb>> PerformPaginatedQueryAsync(
            Cursor<GuissFilter> cursor,
            QueryOperationConfig queryOperationConfig)
        {
            var table = Table.LoadTable(this.amazonDynamoDb, this.tableName);
            var query = table.Query(queryOperationConfig);
            var response = await query.GetNextSetAsync().ConfigureAwait(false);

            while (!query.IsDone && response.Count < cursor.Limit)
            {
                response.AddRange(await query.GetNextSetAsync().ConfigureAwait(false));
            }

            if (cursor.BackwardSearch)
            {
                response.Reverse();
            }

            cursor.LastEvaluatedKey = query.PaginationToken;

            return response.Select(r => this.dynamoDb.FromDocument<GuissDb>(r));
        }
    }
}
