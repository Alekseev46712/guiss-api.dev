using Refinitiv.Aaa.GuissApi.Data.Interfaces;
using Refinitiv.Aaa.GuissApi.Data.Models;
using Refinitiv.Aaa.GuissApi.Interfaces.UserAttribute;
using Refinitiv.Aaa.Pagination.Models;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Refinitiv.Aaa.GuissApi.Data.Constants;
using Refinitiv.Aaa.GuissApi.Interfaces.Configuration;

namespace Refinitiv.Aaa.GuissApi.Data.Repositories
{
    /// <inheritdoc />
    public class UserAttributeRepository : IUserAttributeRepository
    {
        private readonly IDynamoDBContext dynamoDb;
        private readonly AppSettings appSettings;
        private readonly ILogger<UserAttributeRepository> logger;
        private readonly DynamoDBOperationConfig operationConfig;
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
            this.appSettings = appSettings.Value;
            this.logger = logger;
            this.userAttributeQueryWrapper = userAttributeQueryWrapper;
            operationConfig = new DynamoDBOperationConfig { SkipVersionCheck = false,
                OverrideTableName = appSettings.Value.DynamoDb.UserAttributeTableName };
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
                IndexName = GetIndexByFilter(cursor.QueryParamsObject),
                Filter = queryFilter
            };

            var items = await userAttributeQueryWrapper.PerformQueryAsync(appSettings.DynamoDb.UserAttributeTableName, cursor, queryOperationConfig);
            return items;
        }

        /// <inheritdoc />
        public Task<UserAttributeDb> SaveAsync(UserAttributeDb item)
        {
            // stub, will be extended
            return Task.FromResult(item);
        }

        /// <inheritdoc/>
        public async Task DeleteAsync(string userUuid)
        {
            // stub, will be extended
            await Task.CompletedTask;
        }

        private static QueryFilter BuildQueryFilter(UserAttributeFilter appAccountFilter)
        {
            var queryFilter = new QueryFilter();

            if (appAccountFilter == null)
            {
                return queryFilter;
            }

            if (appAccountFilter.UserUuid != null)
            {
                queryFilter.AddCondition(UserAttributeNames.UserUuid,
                    QueryOperator.Equal,
                    appAccountFilter.UserUuid.ToLower(CultureInfo.CurrentCulture));
            }

            if (appAccountFilter.Name != null)
            {
                queryFilter.AddCondition(UserAttributeNames.Name,
                    QueryOperator.Equal,
                    appAccountFilter.Name.ToLower(CultureInfo.CurrentCulture));
            }

            return queryFilter;
        }

        private static string GetIndexByFilter(UserAttributeFilter filter)
        {
            if (filter == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(filter.Name))
            {
                return IndexNames.NameIndex;
            }

            return null;
        }

    }
}
