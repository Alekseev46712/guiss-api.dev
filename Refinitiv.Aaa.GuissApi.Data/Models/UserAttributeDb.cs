using Amazon.DynamoDBv2.DataModel;
using Refinitiv.Aaa.GuissApi.Data.Constants;
using Refinitiv.Aaa.GuissApi.Data.Converters;
using System;

namespace Refinitiv.Aaa.GuissApi.Data.Models
{
    /// <summary>
    /// DTO for UserAttribute.
    /// </summary>
    public class UserAttributeDb
    {
        /// <summary>
        /// Gets or sets the User Uuid.
        /// </summary>
        /// <value>The User Uuid.</value>
        [DynamoDBHashKey]
        public string UserUuid { get; set; }

        /// <summary>
        /// Gets or sets the user attribute name for case-independent searching.
        /// </summary>
        /// <value>The user attribute name converted to lower case.</value>
        [DynamoDBRangeKey]
        [DynamoDBGlobalSecondaryIndexHashKey(IndexNames.NameIndex)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the user attribute name for case-independent searching.
        /// </summary>
        /// <value>The user attribute name converted to lower case.</value>
        public string SearchName { get; set; }

        /// <summary>
        /// Gets or sets the user attribute value.
        /// </summary>
        /// <value>The user attribute value.</value>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets a version number used for optimistic concurrency control.
        /// </summary>
        /// <value>Version number.</value>
        [DynamoDBVersion]
        public long? Version { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the UserAttribute was last modified.
        /// </summary>
        /// <value>The date and time when the UserAttribute was last modified.</value>
        [DynamoDBProperty(typeof(DateTimeOffsetConverter))]
        public DateTimeOffset UpdatedOn { get; set; }

        /// <summary>
        /// Gets or sets the id of the last modifier.
        /// </summary>
        /// <value>The id of the last modifier.</value>
        public string UpdatedBy { get; set; }
    }
}
