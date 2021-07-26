using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Refinitiv.Aaa.GuissApi.Interfaces.Models.UserAttribute
{
    /// <summary>
    /// Model for endpoint
    /// </summary>
    public class UserAttributeDetails
    {
        /// <summary>
        /// Gets or sets the User Uuid for case-independent searching.
        /// </summary>
        /// <value>The User Uuid.</value>
        [Required]
        public string UserUuid { get; set; }

        /// <summary>
        /// Gets or sets the user attribute name.
        /// </summary>
        /// <value>The user attribute name.</value>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the user attribute value.
        /// </summary>
        /// <value>The user attribute value.</value>
        [Required]
        public string Value { get; set; }
    }
}
