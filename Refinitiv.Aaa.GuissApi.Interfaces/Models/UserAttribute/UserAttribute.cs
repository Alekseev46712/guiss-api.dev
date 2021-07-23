using System;
using System.Collections.Generic;
using System.Text;

namespace Refinitiv.Aaa.GuissApi.Interfaces.Models.UserAttribute
{
    public class UserAttribute : UserAttributeDetails
    {
        public UserAttribute()
        {

        }

        public UserAttribute(UserAttributeDetails details) : base(details)
        {

        }

        public string SearchName { get; set; }

        public long? Version { get; set; }

        public DateTimeOffset UpdatedOn { get; set; }

        public string UpdatedBy { get; set; }
    }
}
