using System;
using Amazon.DynamoDBv2.DataModel;
using Refinitiv.Aaa.GuissApi.Data.Constants;

namespace Refinitiv.Aaa.GuissApi.Data.Models
{
    /// <inheritdoc />
    public class GuissDb
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GuissDb"/> class.
        /// </summary>
        public GuissDb()
        {
            Kind = ItemKinds.Guiss;
        }

        /// <summary>
        /// Gets or sets the Guiss Id.
        /// </summary>
        /// <value>The Id.</value>
        [DynamoDBHashKey]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets a version number used for optimistic concurrency control.
        /// </summary>
        /// <value>Version number.</value>
        [DynamoDBVersion]
        public long? Version { get; set; }

        /// <summary>
        /// Gets or sets the last modified date.
        /// </summary>
        /// <value>The Timestamp.</value>
        [DynamoDBIgnore]
        public DateTimeOffset UpdatedOn { get; set; }

        /// <summary>
        /// Gets or sets the ID of the last user to modify the data.
        /// </summary>
        /// <value>The Id of the user that last modified the record.</value>
        public string UpdatedBy { get; set; }

        /// <summary>
        /// Gets or sets the Guiss name.
        /// </summary>
        /// <value>The Guiss Name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the kind of item in the DynamoDB table.
        /// </summary>
        /// <remarks>For Guiss definitions, this is always "TEMPLATE".</remarks>
        /// <value>The Kind key.</value>
        [DynamoDBRangeKey]
        [DynamoDBGlobalSecondaryIndexHashKey("Kind-index")]
        internal string Kind { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the record was last modified,
        /// expressed as seconds since the Unix epoch.
        /// </summary>
        /// <remarks>DynamoDB API does not support DateTimeOffset.</remarks>
        /// <value>Last Modified time.</value>
        internal long LastModifiedUnixSeconds
        {
            get => UpdatedOn.ToUnixTimeSeconds();
            set => UpdatedOn = DateTimeOffset.FromUnixTimeSeconds(value);
        }
    }
}
