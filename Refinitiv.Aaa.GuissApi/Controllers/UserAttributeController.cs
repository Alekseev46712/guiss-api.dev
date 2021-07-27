using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Refinitiv.Aaa.Api.Common;
using Refinitiv.Aaa.Api.Common.Attributes;
using Refinitiv.Aaa.Foundation.ApiClient.Core.Enums;
using Refinitiv.Aaa.Foundation.ApiClient.Interfaces;
using Refinitiv.Aaa.GuissApi.Data.Exceptions;
using Refinitiv.Aaa.GuissApi.Facade.Interfaces;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.UserAttribute;
using Refinitiv.Aaa.Interfaces.Headers;
using Swashbuckle.AspNetCore.Annotations;

namespace Refinitiv.Aaa.GuissApi.Controllers
{
    /// <summary>
    /// Controller used for CRUD operations on UserAttribute items.
    /// </summary>
    [ApiController]
    [AaaHeaders]
    [Route("api/guiss")]
    [Produces("application/json")]
    [SwaggerOperationFilter(typeof(HeaderOperationFilter))]
    public class UserAttributeController : ControllerBase
    {
        private readonly IUserAttributeHelper userAttributeHelper;
        private readonly IUserAttributeValidator userAttributeValidator;
        private readonly ILoggerHelper<UserAttributeController> loggerHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAttributeController"/> class.
        /// </summary>
        /// <param name="userAttributeHelper">Helper used to access the data.</param>
        /// <param name="userAttributeValidator">Validates attributes.</param>
        /// <param name="loggerHelper">provide logging.</param>

        public UserAttributeController(
            IUserAttributeHelper userAttributeHelper,
            IUserAttributeValidator userAttributeValidator,
            ILoggerHelper<UserAttributeController> loggerHelper)
        {
            this.userAttributeHelper = userAttributeHelper;
            this.userAttributeValidator = userAttributeValidator;
            this.loggerHelper = loggerHelper;
        }

        /// <summary>
        /// Gets full list of attributes registered for the specified user.
        /// </summary>
        /// <param name="userUuid">The uuid of the user.</param>
        /// <returns>IActionResult.</returns>
        [HttpGet("getfulluserprofile")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns the full list of the registered attributes")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async Task<OkObjectResult> Get([FromQuery, Required] string userUuid)
        {
            var result = await userAttributeHelper.GetAllByUserUuidAsync(userUuid);

            return Ok(result);
        }


        /// <summary>
        /// Deletes user attributes by Uuis and by Name.
        /// </summary>
        /// <param name="uuid">The attributes uuid to be deleted.</param>
        /// <param name="name">The attributes name to be deleted.</param>
        /// <returns>IActionResult</returns>
        [HttpDelete("{uuid}/{name}")]
        [SwaggerResponse((int)StatusCodes.Status200OK, "OK")]
        [SwaggerResponse((int)StatusCodes.Status400BadRequest, "Bad Request")]
        [SwaggerResponse((int)StatusCodes.Status404NotFound, "Attribute does not exist")]
        [SwaggerResponse((int)StatusCodes.Status500InternalServerError, "Internal Server Error")]
        public async Task<IActionResult> DeleteUserAttribute([FromRoute] string uuid, [FromRoute] string name)
        {
            // Check that the model exists
            var deletedItem = await userAttributeHelper.GetUserAttributeAsync(uuid, name);
            if (deletedItem == null)
            {
                return NotFound();
            }

            // Now delete the model
            await userAttributeHelper.DeleteUserAttributeAsync(uuid, name);

            return NoContent();
        }

        /// <summary>
        /// Creates and updates user attributes
        /// </summary>
        /// <param name="details">The uuid of the user.</param>
        /// <returns>IActionResult.</returns>
        [HttpPut]
        [SwaggerResponse(StatusCodes.Status200OK, "UserAttribute created or updated")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User with the specified Uuid not found")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request, validation error")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Application Account has been updated by someone else")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> Put([FromBody, Required] UserAttributeDetails details)
        {
            var attributeValidationResult = await userAttributeValidator.ValidateAttributeAsync(details);

            if (!(attributeValidationResult is AcceptedResult))
            {
                return attributeValidationResult;
            }

            var userAttribute = await userAttributeValidator.ValidatePutRequestAsync(details);

            if (userAttribute != null)
            {
                try
                {
                    var updatedAttribute = await userAttributeHelper.UpdateAsync(userAttribute, details.Value);
                    loggerHelper.LogAuditEntry(LoggerEvent.Updated, "Attribute Updated", $"uuid :{updatedAttribute.UserUuid}, name : {updatedAttribute.Name}");
                    return Ok(updatedAttribute);
                }
                catch (UpdateConflictException)
                {
                    return Conflict();
                }           
            }
      
            var savedItem = await userAttributeHelper.InsertAsync(details);
            loggerHelper.LogAuditEntry(LoggerEvent.Created, "Attribute Created", $"uuid :{savedItem.UserUuid}, name : {savedItem.Name}");

            return Ok(savedItem);
        }
    }
}
