using Microsoft.AspNetCore.Mvc;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.UserAttribute;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Refinitiv.Aaa.GuissApi.Facade.Interfaces
{
    public interface IUserAttributeValidator
    {
        Task<IActionResult> ValidateAttributeAsync(UserAttribute newUserAttributeDetails);
        Task<UserAttribute> ValidatePutRequestAsync(UserAttribute userAttribute);
    }
}
