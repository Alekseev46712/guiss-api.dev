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
        /// The UserUuid attribute.
        /// </summary>
        public const string UserUuid = nameof(UserAttributeDb.UserUuid);

        /// <summary>
        /// The Name attribute.
        /// </summary>
        public const string Name = nameof(UserAttributeDb.Name);

        /// <summary>
        /// The SearchName attribute.
        /// </summary>
        public const string SearchName = nameof(UserAttributeDb.SearchName);

        /// <summary>
        /// The Namespace attribute.
        /// </summary>
        public const string Namespace = nameof(UserAttributeDb.Namespace);
    }
}
