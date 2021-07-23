using System.Diagnostics.CodeAnalysis;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.Extensions.Logging;
using Refinitiv.Aaa.GuissApi.Data.Interfaces;
using Refinitiv.Aaa.GuissApi.Data.Models;
using Refinitiv.Aaa.GuissApi.Interfaces.UserAttribute;

namespace Refinitiv.Aaa.GuissApi.Data.Repositories
{
    /// <summary>
    /// A wrapper for the DynamoDB Document API.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class UserAttributeQueryWrapper: BaseQueryWrapper<UserAttributeDb, UserAttributeFilter>, IDynamoDbDocumentQueryWrapper<UserAttributeDb, UserAttributeFilter>
    {
        /// <inheritdoc />
        public UserAttributeQueryWrapper(IAmazonDynamoDB amazonDynamoDb, IDynamoDBContext dynamoDb, ILogger<UserAttributeQueryWrapper> logger)
            : base(amazonDynamoDb, dynamoDb, logger)
        {
        }
    }
}
