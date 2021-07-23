using System;
using System.Collections.Generic;
using System.Text;

namespace Refinitiv.Aaa.GuissApi.Interfaces.Models.UserAttribute
{
    public class UserAttributeDetails
    {
        public UserAttributeDetails()
        {

        }

        public UserAttributeDetails(UserAttributeDetails details)
        {
            this.UserUuid = details.UserUuid;
            this.Name = details.Name;
            this.Value = details.Value;

        }

        public string UserUuid { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
