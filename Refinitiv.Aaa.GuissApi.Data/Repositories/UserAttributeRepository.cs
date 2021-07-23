using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Refinitiv.Aaa.GuissApi.Data.Constants;
using Refinitiv.Aaa.GuissApi.Data.Exceptions;
using Refinitiv.Aaa.GuissApi.Data.Interfaces;
using Refinitiv.Aaa.GuissApi.Data.Models;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.Configuration;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.UserAttribute;
using Refinitiv.Aaa.Pagination.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Refinitiv.Aaa.GuissApi.Data.Repositories
{
    /// <inheritdoc />
    public class UserAttributeRepository : IUserAttributeRepository
    {
        private readonly IDynamoDBContext dynamoDb;
        private readonly DynamoDBOperationConfig dbConfig;
        private readonly ILogger<UserAttributeRepository> logger;
        private readonly IDynamoDbDocumentQueryWrapper<UserAttributeDb, UserAttributeFilter> userAttributeQueryWrapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAttributeRepository"/> class.
        /// </summary>
        /// <param name="dynamoDb">Dynamodb instance.</param>
        /// <param name="appSettings">Database configuration.</param>
        /// <param name="userAttributeQueryWrapper">Query wrapper.</param>
        /// <param name="logger">Logger to write errors to.</param>
        public UserAttributeRepository(
            IDynamoDBContext dynamoDb,
            IOptions<AppSettings> appSettings,
            IDynamoDbDocumentQueryWrapper<UserAttributeDb, UserAttributeFilter> userAttributeQueryWrapper,
            ILogger<UserAttributeRepository> logger)
        {
            this.dynamoDb = dynamoDb;
            this.logger = logger;
            this.userAttributeQueryWrapper = userAttributeQueryWrapper;
            dbConfig = new DynamoDBOperationConfig { OverrideTableName = appSettings.Value.DynamoDb.UserAttributeTableName };
        }

        /// <inheritdoc />
        public async Task<IEnumerable<UserAttributeDb>> SearchAsync(UserAttributeFilter filter)
        {
            var cursor = new Cursor<UserAttributeFilter>(0, filter);

            var queryFilter = BuildQueryFilter(cursor.QueryParamsObject);

            var queryOperationConfig = new QueryOperationConfig
            {
                BackwardSearch = cursor.BackwardSearch,
                PaginationToken = cursor.LastEvaluatedKey ?? "{}",
                IndexName = IndexNames.UserUuidIndex,
                Filter = queryFilter
            };

            var (items, _, _) = await userAttributeQueryWrapper.PerformQueryAsync(dbConfig.OverrideTableName, cursor, queryOperationConfig);
            return items;
        }

        /// <inheritdoc />
        public Task<UserAttributeDb> FindByUserUuidAndNameAsync(string userUuid, string name)
        {
            if (userUuid == null)
            {
                throw new ArgumentNullException(nameof(userUuid));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(userUuid));
            }

            return GetUserAttributeByByUserUuidAndNameAsync(userUuid, name);
        }

        /// <inheritdoc />
        public Task<(IEnumerable<UserAttributeDb>, string FirstItemToken, string LastItemToken)> SearchAsync(Cursor<UserAttributeFilter> cursor)
        {
            if (cursor == null)
            {
                throw new ArgumentNullException(nameof(cursor));
            }

            return PerformSearchAsync(cursor);
        }

        /// <inheritdoc />
        public Task<UserAttributeDb> SaveAsync(UserAttributeDb item)
        {
            if (item != null)
            {
                return PerformSaveAsync(item);
            }

            logger.LogError("UserAttributeDb is null when saving item in UserAttributeRepository.");
            throw new ArgumentNullException(nameof(item));
        }

        /// <inheritdoc />
        public async Task DeleteAsync(string userUuid, string name)
        {
            if (userUuid == null)
            {
                logger.LogError("UserUuid is null when deleting item in UserAttributeRepository.");

                throw new ArgumentNullException(nameof(userUuid));
            }

            if (name == null)
            {
                logger.LogError("Name is null when deleting item in UserAttributeRepository.");

                throw new ArgumentNullException(nameof(name));
            }

            await PerformDeleteAsync(userUuid, name);
        }

        private async Task<(IEnumerable<UserAttributeDb>, string FirstItemToken, string LastItemToken)> PerformSearchAsync(Cursor<UserAttributeFilter> cursor)
        {
            var queryFilter = BuildQueryFilter(cursor.QueryParamsObject);

            var queryOperationConfig = new QueryOperationConfig
            {
                BackwardSearch = cursor.BackwardSearch,
                PaginationToken = cursor.LastEvaluatedKey ?? "{}",
                IndexName = IndexNames.UserUuidIndex,
                Filter = queryFilter,
                Limit = cursor.Limit,
            };

            var (items, firstItemToken, lastItemToken) = await userAttributeQueryWrapper.PerformQueryAsync(dbConfig.OverrideTableName, cursor, queryOperationConfig);
            return (items, firstItemToken, lastItemToken);
        }

        private async Task<UserAttributeDb> GetUserAttributeByByUserUuidAndNameAsync(string userUuid, string name)
        {
            try
            {
                return await dynamoDb.LoadAsync<UserAttributeDb>(userUuid, name, dbConfig);
            }
            catch (AmazonDynamoDBException ex)
            {
                logger.LogError(ex, $"{nameof(GetUserAttributeByByUserUuidAndNameAsync)}: An exception has occurred while get UserAttribute with userUuid {userUuid} and name {name}.");
                throw;
            }
        }

        private async Task<UserAttributeDb> PerformSaveAsync(UserAttributeDb item)
        {
            try
            {
                await dynamoDb.SaveAsync(item, dbConfig);
            }
            catch (ConditionalCheckFailedException e)
            {
                logger.LogTrace("Exception caught: {Message}", e.InnerException?.Message ?? e.Message);
                throw new UpdateConflictException(e);
            }

            // Reload to get the updated version number.
            var savedItem = await FindByUserUuidAndNameAsync(item.UserUuid, item.Name);
            if (savedItem == null || (item.Version != null && item.Version == savedItem.Version))
            {
                // If DynamoDB is quite ready; try a consistent read.
                logger.LogWarning($"Get item by userUuid {item.UserUuid} and {item.Name} returned null");
                return await GetItemByUserUuidAndNameConsistentReadAsync(item.UserUuid, item.Name);
            }

            return savedItem;
        }

        private async Task<UserAttributeDb> GetItemByUserUuidAndNameConsistentReadAsync(string userUuid, string name)
        {
            var operationConfig = dbConfig;
            operationConfig.ConsistentRead = true;
            return await dynamoDb.LoadAsync<UserAttributeDb>(userUuid, name, operationConfig);
        }

        private async Task PerformDeleteAsync(string userUuid, string name)
        {
            var item = await FindByUserUuidAndNameAsync(userUuid, name);

            if (item == null)
            {
                logger.LogWarning($"Attempt to delete non-existent user attribute with userUuid {userUuid} and name {name}.");
                return;
            }

            // hard delete
            await dynamoDb.DeleteAsync(item, dbConfig);

            logger.LogTrace($"Deleted item with UserUuid: {userUuid} and Name {name}.");
        }

        private static QueryFilter BuildQueryFilter(UserAttributeFilter userAttributeFilter)
        {
            var queryFilter = new QueryFilter();

            if (userAttributeFilter == null)
            {
                return queryFilter;
            }

            if (userAttributeFilter.UserUuid != null)
            {
                queryFilter.AddCondition(UserAttributeNames.SearchUserUuid,
                    QueryOperator.Equal,
                    userAttributeFilter.UserUuid.ToLower(CultureInfo.CurrentCulture));
            }

            if (userAttributeFilter.Names != null && userAttributeFilter.Names.Any())
            {
                queryFilter.AddCondition(UserAttributeNames.SearchName,
                    ScanOperator.In,
                    userAttributeFilter.Names.Select(n => new AttributeValue(n.ToLower(CultureInfo.CurrentCulture))).ToList());
            }

            return queryFilter;
        }
    }
}
