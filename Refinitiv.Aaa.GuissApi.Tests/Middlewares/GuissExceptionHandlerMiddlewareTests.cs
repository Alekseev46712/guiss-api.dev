using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Refinitiv.Aaa.Foundation.ApiClient.Exceptions;
using Refinitiv.Aaa.GuissApi.Facade.Exceptions;
using Refinitiv.Aaa.GuissApi.Middlewares;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Refinitiv.Aaa.GuissApi.Tests.Middlewares
{
    [TestFixture]
    class GuissExceptionHandlerMiddlewareTests
    {
        private Mock<ILogger<GuissExceptionHandlerMiddleware>> mockLogger;
        private DefaultHttpContext httpContext;

        [SetUp]
        public void Setup()
        {
            httpContext = new DefaultHttpContext();
            mockLogger = new Mock<ILogger<GuissExceptionHandlerMiddleware>>();
        }

        [Test]
        public async Task GuissExceptionHandlerMiddleware_ReturnsBadRequest_OnArgumentException()
        {
            //Arrange
            var excpectedException = new ArgumentException();
            RequestDelegate next = (HttpContext) =>
            {
                return Task.FromException(excpectedException);
            };

            var middleware = new GuissExceptionHandlerMiddleware(next, mockLogger.Object);

            //Act
            await middleware.InvokeAsync(httpContext);

            //Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, (HttpStatusCode)httpContext.Response.StatusCode);
        }

        [Test]
        public async Task GuissExceptionHandlerMiddleware_ReturnsBadRequest_OnInvalidPaginationTokenException()
        {
            //Arrange
            var excpectedException = new InvalidPaginationTokenException();
            RequestDelegate next = (HttpContext) =>
            {
                return Task.FromException(excpectedException);
            };

            var middleware = new GuissExceptionHandlerMiddleware(next, mockLogger.Object);

            //Act
            await middleware.InvokeAsync(httpContext);

            //Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, (HttpStatusCode)httpContext.Response.StatusCode);
        }

        [Test]
        public async Task GuissExceptionHandlerMiddleware_ReturnsForbidden_OnInvalidHttpResponseExceptionWithStatusCodeForbidden()
        {
            //Arrange
            var testString = "test";
            var excpectedException = new InvalidHttpResponseException(testString, HttpStatusCode.Forbidden, testString);
            RequestDelegate next = (HttpContext) =>
            {
                return Task.FromException(excpectedException);
            };

            var middleware = new GuissExceptionHandlerMiddleware(next, mockLogger.Object);

            //Act
            await middleware.InvokeAsync(httpContext);

            //Assert
            Assert.AreEqual(HttpStatusCode.Forbidden, (HttpStatusCode)httpContext.Response.StatusCode);
        }

        [Test]
        public async Task GuissExceptionHandlerMiddleware_ReturnsInternalServerError_OnInvalidHttpResponseException()
        {
            //Arrange
            var testString = "test";
            var excpectedException = new InvalidHttpResponseException(testString, HttpStatusCode.InternalServerError, testString);
            RequestDelegate next = (HttpContext) =>
            {
                return Task.FromException(excpectedException);
            };

            var middleware = new GuissExceptionHandlerMiddleware(next, mockLogger.Object);

            //Act
            await middleware.InvokeAsync(httpContext);

            //Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, (HttpStatusCode)httpContext.Response.StatusCode);
        }

        [Test]
        public async Task GuissExceptionHandlerMiddleware_ReturnsBadRequest_OnHttpRequestException()
        {
            //Arrange
            var excpectedException = new HttpRequestException();
            RequestDelegate next = (HttpContext) =>
            {
                return Task.FromException(excpectedException);
            };

            var middleware = new GuissExceptionHandlerMiddleware(next, mockLogger.Object);

            //Act
            await middleware.InvokeAsync(httpContext);

            //Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, (HttpStatusCode)httpContext.Response.StatusCode);
        }

        [Test]
        public async Task GuissExceptionHandlerMiddleware_ReturnsInternalServerError_OnException()
        {
            //Arrange
            var excpectedException = new Exception();
            RequestDelegate next = (HttpContext) =>
            {
                return Task.FromException(excpectedException);
            };

            var middleware = new GuissExceptionHandlerMiddleware(next, mockLogger.Object);

            //Act
            await middleware.InvokeAsync(httpContext);

            //Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, (HttpStatusCode)httpContext.Response.StatusCode);
        }
    }
}
