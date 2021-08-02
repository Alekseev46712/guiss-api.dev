using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Refinitiv.Aaa.GuissApi.Facade.Helpers
{
    /// <summary>
    /// Is intended to perform base logic for http requests.
    /// </summary>
    public class GuissDelegatingHandler : DelegatingHandler
    {
        private readonly ILogger<GuissDelegatingHandler> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuissDelegatingHandler"/> class.
        /// </summary>
        /// <param name="logger">Logger for <see cref="GuissDelegatingHandler"/> class.</param>
        public GuissDelegatingHandler(ILogger<GuissDelegatingHandler> logger)
        {
            this.logger = logger;
        }

        /// <inheritdoc />
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage httpResponseMessage;

            try
            {
                logger.LogDebug($"Executing request: {request.RequestUri}");
                httpResponseMessage = await base.SendAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to request {request?.RequestUri}.");
                throw;
            }

            try
            {
                httpResponseMessage.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                logger.LogError(ex, ex.Message + $" Requested URI: {request?.RequestUri}.");
            }

            return httpResponseMessage;
        }
    }
}
