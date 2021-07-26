using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Refinitiv.Aaa.GuissApi.Interfaces.Models.UserAttribute
{
    public class UserAttributeDetails
    {
        public UserAttributeDetails(UserAttributeDetails details)
        {
            this.UserUuid = details.UserUuid;
            this.Name = details.Name;
            this.Value = details.Value;

        }

        [Required]
        public string UserUuid { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Value { get; set; }
    }
}
