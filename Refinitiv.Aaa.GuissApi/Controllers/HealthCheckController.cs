using System.Net;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Refinitiv.Aaa.GuissApi.Controllers
{
    /// <summary>
    /// Controller for API HealthCheck endpoint.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class HealthCheckController : ControllerBase
    {
        /// <summary>
        /// Get API HealthCheck.
        /// </summary>
        /// <returns>Version of the API and related status.</returns>
        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK, "HealthCheck")]
        public IActionResult HealthCheck()
        {
            return Ok();
        }
    }
}
