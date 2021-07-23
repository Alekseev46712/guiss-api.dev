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
        private readonly IAaaRequestHeaders aaaRequestHeaders;
        private readonly IUserAttributeValidator userAttributeValidator;
        private readonly ILoggerHelper<UserAttributeController> loggerHelper;



        /// <summary>
        /// Initializes a new instance of the <see cref="UserAttributeController"/> class.
        /// </summary>
        /// <param name="userAttributeHelper">Helper used to access the data.</param>
        /// <param name="aaaRequestHeaders">Helps to recieve headers.</param>
        /// <param name="userAttributeValidator">Helps to validate attributes.</param>

        public UserAttributeController(
            IUserAttributeHelper userAttributeHelper,
            IAaaRequestHeaders aaaRequestHeaders,
            IUserAttributeValidator userAttributeValidator,
            ILoggerHelper<UserAttributeController> loggerHelper)
        {
            this.userAttributeHelper = userAttributeHelper;
            this.aaaRequestHeaders = aaaRequestHeaders;
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
            // Create object containing all required properties for the create
            var userAttribute = new UserAttribute(details)
            {
                UpdatedOn = DateTime.UtcNow,
                UpdatedBy = aaaRequestHeaders.RefinitivUuid,
                SearchName = details.Name.ToLower()
            };

            var attributeValidationResult = await userAttributeValidator.ValidateAttributeAsync(userAttribute);

            if (!(attributeValidationResult is AcceptedResult))
            {
                return attributeValidationResult;
            }

            var putRequestValidationResult = await userAttributeValidator.ValidatePutRequestAsync(userAttribute);

            if (putRequestValidationResult != null)
            {
                try
                {
                    var updatedAttribute = await userAttributeHelper.UpdateAsync(putRequestValidationResult);
                    loggerHelper.LogAuditEntry(LoggerEvent.Updated, "Attribute Updated", $"uuid :{updatedAttribute.UserUuid}, name : {updatedAttribute.Name}");
                    return Ok(updatedAttribute);
                }
                catch (UpdateConflictException)
                {
                    return Conflict();
                }
                
            }
      
            var savedItem = await userAttributeHelper.InsertAsync(userAttribute);
            loggerHelper.LogAuditEntry(LoggerEvent.Created, "Attribute Created", $"uuid :{savedItem.UserUuid}, name : {savedItem.Name}");

            return Ok(savedItem);

        }
    }
}
