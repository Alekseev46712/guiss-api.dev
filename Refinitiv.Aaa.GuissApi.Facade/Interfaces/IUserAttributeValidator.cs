using Microsoft.AspNetCore.Mvc;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.UserAttribute;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Refinitiv.Aaa.GuissApi.Facade.Interfaces
{
    /// <summary>
    /// Validets requests
    /// </summary>
    public interface IUserAttributeValidator
    {
        /// <summary>
        /// Checks if User Uuid is valid by calling users api
        /// </summary>
        /// <param name="newUserAttributeDetails">User Attribute Details.</param>
        /// <returns>IActionResult.</returns>
        Task<IActionResult> ValidateAttributeAsync(UserAttribute newUserAttributeDetails);

        /// <summary>
        /// Checks if it's update request or post
        /// </summary>
        /// <param name="userAttribute">User Attribute Details.</param>
        /// <returns>UserAttribute or null.</returns>
        Task<UserAttribute> ValidatePutRequestAsync(UserAttribute userAttribute);
    }
}
