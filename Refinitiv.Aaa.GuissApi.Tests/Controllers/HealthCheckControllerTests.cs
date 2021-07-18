using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Refinitiv.Aaa.GuissApi.Controllers;

namespace Refinitiv.Aaa.GuissApi.Tests.Controllers
{
    public class HealthCheckControllerTests
    {
        private HealthCheckController _fixture;
        private readonly Type _fixtureType = typeof(HealthCheckController);

        [SetUp]
        public void Init()
        {
            _fixture = new HealthCheckController();
        }

        [Test]
        public void Class_HasApiControllerAttribute()
        {
            Assert.That(_fixtureType, Has.Attribute<ApiControllerAttribute>());
        }

        [Test]
        public void HealthCheck_Returns_ServiceStatus()
        {
            var result = _fixture.HealthCheck();

            Assert.IsInstanceOf<OkResult>(result);
        }

        [Test]
        public void HealthCheck_Timestamp_NotNull()
        {
            var result = _fixture.HealthCheck() as OkResult;

            Assert.IsNotNull(result);
            Assert.AreEqual((int)HttpStatusCode.OK, result.StatusCode);
        }
    }
}
