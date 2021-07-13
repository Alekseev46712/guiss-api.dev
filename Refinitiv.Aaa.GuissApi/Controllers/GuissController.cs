using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Refinitiv.Aaa.Api.Common;
using Refinitiv.Aaa.Api.Common.Attributes;
using Refinitiv.Aaa.Foundation.ApiClient.Core.Enums;
using Refinitiv.Aaa.Foundation.ApiClient.Interfaces;
using Refinitiv.Aaa.Interfaces.Business;
using Refinitiv.Aaa.GuissApi.Data.Exceptions;
using Refinitiv.Aaa.GuissApi.Facade.Exceptions;
using Refinitiv.Aaa.GuissApi.Facade.Interfaces;
using Refinitiv.Aaa.GuissApi.Facade.Models;
using Swashbuckle.AspNetCore.Annotations;
using GuissFilter = Refinitiv.Aaa.GuissApi.Data.Models.GuissFilter;

namespace Refinitiv.Aaa.GuissApi.Controllers
{
    /// <summary>
    /// Controller used for CRUD operations on Guiss items.
    /// </summary>
    [ApiController]
    [AaaHeaders]
    [Route("api/guiss")]
    [Produces("application/json")]
    [SwaggerOperationFilter(typeof(HeaderOperationFilter))]
    public class GuissController : ControllerBase
    {
        private readonly IGuissHelper templateHelper;
        private readonly ILoggerHelper<GuissController> loggerHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuissController"/> class.
        /// </summary>
        /// <param name="templateHelper">Helper used to access the data.</param>
        /// <param name="loggerHelper">Logger helper used to write logs.</param>
        public GuissController(
            IGuissHelper templateHelper,
            ILoggerHelper<GuissController> loggerHelper)
        {
            this.templateHelper = templateHelper;
            this.loggerHelper = loggerHelper;
        }

        /// <summary>
        /// Gets all templates.
        /// </summary>
        /// <param name="filter">Filter criteria.</param>
        /// <param name="limit">The limit value.</param>
        /// <param name="pagination">The pagination value (this is url encoded).</param>
        /// <returns>IActionResult.</returns>
        [HttpGet(Name = "GetGuiss")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Guiss", typeof(IResultSet<Guiss>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Guiss does not exist")]
        public async Task<IActionResult> Get([FromQuery] GuissFilter filter = null, [FromQuery] int? limit = null, [FromQuery] string pagination = null)
        {
            try
            {
                var paging = new PaginationModel
                {
                    Pagination = pagination,
                };

                if (limit < 1)
                {
                    return new BadRequestResult();
                }

                if (limit.HasValue)
                {
                    paging.Limit = limit.Value;
                }

                var result = await templateHelper.FindAllAsync(filter, paging);

                return new JsonResult(result);
            }
            catch (InvalidPaginationTokenException)
            {
                return new BadRequestResult();
            }
        }

        /// <summary>
        /// Retrieves a model with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the model to be retrieved.</param>
        /// <returns>IActionResult.</returns>
        [HttpGet("{id}", Name = "GetGuissById")]
        [SwaggerResponse((int)HttpStatusCode.OK, "The model", typeof(Guiss))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Guiss does not exist")]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            // First get the model
            var template = await templateHelper.FindByIdAsync(id);

            // If the model doesn't exist then return NotFound
            if (template == null)
            {
                return NotFound();
            }

            // Return the model
            return new JsonResult(template);
        }

        /// <summary>
        /// Creates a new model.
        /// </summary>
        /// <param name="newGuiss">Details of the model to be created.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost]
        [SwaggerResponse((int)HttpStatusCode.Created, "Guiss created", typeof(Guiss))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad Request, validation error")]
        [SwaggerResponse((int)HttpStatusCode.PreconditionFailed, "Validation error", typeof(Dictionary<string, string[]>))]
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

        /// <summary>
        /// Updates an existing model.
        /// </summary>
        /// <param name="id">The model ID to be updated.</param>
        /// <param name="template">The updated model details.</param>
        /// <returns>IActionResult.</returns>
        [HttpPut("{id}")]
        [SwaggerResponse((int)HttpStatusCode.Created, "Guiss updated", typeof(Guiss))]
        [SwaggerResponse((int)HttpStatusCode.PreconditionFailed, "Validation error", typeof(Dictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Guiss with the specified ID does not exist")]
        [SwaggerResponse((int)HttpStatusCode.Conflict, "Guiss has been updated by someone else")]
        public async Task<IActionResult> Put([FromRoute] string id, [FromBody] Guiss template)
        {
            if (template == null)
            {
                return NotFound();
            }

            // Check that the ID in the route is the same as the Guiss ID
            if (id != template.Id)
            {
                return StatusCode((int)HttpStatusCode.PreconditionFailed);
            }

            // Check that the model exists
            if (await templateHelper.FindByIdAsync(id) == null)
            {
                return NotFound();
            }

            // Now update the model
            Guiss savedItem;

            try
            {
                savedItem = await templateHelper.UpdateAsync(template);
            }
            catch (UpdateConflictException)
            {
                return Conflict();
            }

            // Log audit entry
            loggerHelper.LogAuditEntry(LoggerEvent.Updated, "Guiss Updated", savedItem.Id);

            return Ok(savedItem);
        }

        /// <summary>
        /// Deletes an existing template.
        /// </summary>
        /// <param name="id">The template Id to be deleted.</param>
        /// <returns>IActionResult.</returns>
        [HttpDelete("{id}")]
        [SwaggerResponse((int)HttpStatusCode.NoContent, "Guiss deleted")]
        [SwaggerResponse((int)HttpStatusCode.PreconditionFailed, "Validation error", typeof(Dictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Guiss with the specified ID does not exist")]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            // Check that the model exists
            var deletedItem = await templateHelper.FindByIdAsync(id);
            if (deletedItem == null)
            {
                return NotFound();
            }

            // Now delete the model
            await templateHelper.DeleteAsync(id);

            // Log audit entry
            loggerHelper.LogAuditEntry(LoggerEvent.Deleted, "Guiss Deleted", deletedItem.Id);

            return NoContent();
        }
    }
}
