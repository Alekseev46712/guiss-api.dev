using System;
using System.Collections.Generic;
using System.Text;

namespace Refinitiv.Aaa.GuissApi.Interfaces.Models.UserAttribute
{
    /// <summary>
    /// UserAttribute model.
    /// </summary>
    public class UserAttribute : UserAttributeDetails
    {
        /// <summary>
        /// Gets or sets a version number used for optimistic concurrency control.
        /// </summary>
        /// <value>Version number.</value>
        public long? Version { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the UserAttribute was last modified.
        /// </summary>
        /// <value>The date and time when the UserAttribute was last modified.</value
        public DateTimeOffset UpdatedOn { get; set; }

        /// <summary>
        /// Gets or sets the id of the last modifier.
        /// </summary>
        /// <value>The id of the last modifier.</value>
        public string UpdatedBy { get; set; }
    }
}
