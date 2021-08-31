using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Refinitiv.Aaa.Foundation.ApiClient.Constants;
using Refinitiv.Aaa.Foundation.ApiClient.Exceptions;
using Refinitiv.Aaa.GuissApi.Facade.Exceptions;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Refinitiv.Aaa.GuissApi.Middlewares
{
    /// <summary>
    /// Web API Middleware responsible for handling uncaught exceptions.
    /// </summary>
    public sealed class GuissExceptionHandlerMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<GuissExceptionHandlerMiddleware> logger;

        private const string FailedToRequestMessage = "Failed to request external service";
        private const string InternalErrorMessage = "Failed to proccess the request due to internal error";

        /// <summary>
        /// Initializes a new instance of the <see cref="GuissExceptionHandlerMiddleware" /> class.
        /// </summary>
        /// <param name="next">The request pipeline.</param>
        /// <param name="logger">The logging information.</param>
        public GuissExceptionHandlerMiddleware(
            RequestDelegate next,
            ILogger<GuissExceptionHandlerMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        /// <summary>
        /// Invokes the next request delegate an catches any exceptions.
        /// </summary>
        /// <param name="context">The information about the HTTP request.</param>
        /// <returns>A Task.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (ArgumentException ex)
            {
                await ProcessDefaultErrorAsync(context, ex, ex.Message, HttpStatusCode.BadRequest);
            }
            catch (InvalidPaginationTokenException ex)
            {
                await ProcessDefaultErrorAsync(context, ex, ex.Message, HttpStatusCode.BadRequest);
            }
            catch (InvalidHttpResponseException ex) when (ex.StatusCode == HttpStatusCode.Forbidden)
            {
                await ProcessDefaultErrorAsync(context, ex, ex.Message, HttpStatusCode.Forbidden);   
            }
            catch (InvalidHttpResponseException ex)
            {
                await ProcessDefaultErrorAsync(context, ex, ex.Message, HttpStatusCode.InternalServerError);
            }
            catch (HttpRequestException ex)
            {
                await ProcessDefaultErrorAsync(context, ex, FailedToRequestMessage, HttpStatusCode.BadRequest);
            }
            catch (NotFoundException ex)
            {
                await ProcessDefaultErrorAsync(context, ex, ex.Message, HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                await ProcessCriticalErrorAsync(context, ex, InternalErrorMessage);
            }
        }

        private async Task ProcessCriticalErrorAsync(HttpContext httpContext, Exception exception, string message)
        {
            logger.LogCritical(exception, exception.Message);
            await WriteErrorResponseAsync(httpContext, message, HttpStatusCode.InternalServerError);
        }

        private async Task ProcessDefaultErrorAsync(HttpContext httpContext, Exception exception, string message, HttpStatusCode statusCode)
        {
            logger.LogError(exception, exception.Message);
            await WriteErrorResponseAsync(httpContext, message, statusCode);
        }

        private async Task WriteErrorResponseAsync(HttpContext httpContext, string message, HttpStatusCode statusCode)
        {
            httpContext.Response.ContentType = MimeTypeNames.Application.ProblemJson;
            httpContext.Response.StatusCode = (int)statusCode;

            var problem = new ProblemDetails
            {
                Status = (int)statusCode,
                Title = message
            };

            var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;
            problem.Extensions["traceId"] = traceId;

            var stream = httpContext.Response.Body;
            await JsonSerializer.SerializeAsync(stream, problem);
        }
    }
}
