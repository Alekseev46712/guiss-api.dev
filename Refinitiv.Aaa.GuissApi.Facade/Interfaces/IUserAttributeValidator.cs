using Microsoft.AspNetCore.Mvc;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.UserAttribute;
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
        /// <param name="userAttributeDetails">User Attribute Model.</param>
        /// <returns>IActionResult.</returns>
        Task<IActionResult> ValidateAttributeAsync(UserAttributeDetails userAttributeDetails);

        /// <summary>
        /// Checks if User Uuid is valid by calling users api
        /// </summary>
        /// <param name="userUuid">User UUID.</param>
        /// <returns>IActionResult.</returns>
        Task<IActionResult> ValidateUserUuidAsync(string userUuid);

        /// <summary>
        /// Checks if it's update request or post
        /// </summary>
        /// <param name="userAttributeDetails">User Attribute Model.</param>
        /// <returns>UserAttribute or null.</returns>
        Task<UserAttribute> ValidatePutRequestAsync(UserAttributeDetails userAttributeDetails);

        /// <summary>
        /// Checks if attributes string contains any comma separated values
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        IActionResult ValidateAttributesString(string attributes);

        /// <summary>
        /// Check exist user attribites by Uuid and Name
        /// </summary>
        /// <param name="userUuid"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<IActionResult> ValidateUserAttributesAsync(string userUuid, string name);

        /// <summary>
        /// Checks if namespaces string contains any comma separated values
        /// </summary>
        /// <param name="namespaces"></param>
        /// <returns></returns>
        IActionResult ValidateNamespacesString(string namespaces);
    }
}
