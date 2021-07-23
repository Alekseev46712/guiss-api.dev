using System.Diagnostics.CodeAnalysis;
using Refinitiv.Aaa.GuissApi.Data.Models;

namespace Refinitiv.Aaa.GuissApi.Data.Constants
{
    /// <summary>
    /// Constants for the names of the attributes in DynamoDB.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class UserAttributeNames
    {
        /// <summary>
        /// The SearchUserUuid attribute.
        /// </summary>
        public const string SearchUserUuid = nameof(UserAttributeDb.SearchUserUuid);

        /// <summary>
        /// The SearchName attribute.
        /// </summary>
        public const string SearchName = nameof(UserAttributeDb.SearchName);
    }
}
