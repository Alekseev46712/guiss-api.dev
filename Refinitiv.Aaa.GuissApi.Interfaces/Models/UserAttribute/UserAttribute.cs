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
        public long? Version { get; set; }

        public DateTimeOffset UpdatedOn { get; set; }

        public string UpdatedBy { get; set; }
    }
}
