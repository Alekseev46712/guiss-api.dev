using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Refinitiv.Aaa.Api.Common;
using Refinitiv.Aaa.Api.Common.Attributes;
using Refinitiv.Aaa.GuissApi.Facade.Interfaces;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAttributeController"/> class.
        /// </summary>
        /// <param name="userAttributeHelper">Helper used to access the data.</param>
        public UserAttributeController(
            IUserAttributeHelper userAttributeHelper)
        {
            this.userAttributeHelper = userAttributeHelper;
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

        [HttpPost]
      
        public async Task<IActionResult> Post([FromBody] GuissDetails newGuiss)
        {
            // Create object containing all required properties for the create
            var template = new Guiss(newGuiss)
            {
                UpdatedOn = DateTimeOffset.UtcNow,
            };

            // Call the helper to insert the new item
            var savedItem = await templateHelper.InsertAsync(template);

            // Log audit entry
            loggerHelper.LogAuditEntry(LoggerEvent.Created, "Guiss Created", savedItem.Id);

            // Return the newly created item
            return CreatedAtAction("Get", savedItem.Id, savedItem);
        }
    }
}
