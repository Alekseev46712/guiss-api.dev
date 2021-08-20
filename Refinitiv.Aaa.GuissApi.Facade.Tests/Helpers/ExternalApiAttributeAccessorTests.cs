using AutoFixture;
using FluentAssertions;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Refinitiv.Aaa.Foundation.ApiClient.Interfaces;
using Refinitiv.Aaa.GuissApi.Facade.Exceptions;
using Refinitiv.Aaa.GuissApi.Facade.Helpers;
using Refinitiv.Aaa.GuissApi.Facade.Interfaces;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.Configuration;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.UserAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Refinitiv.Aaa.GuissApi.Facade.Tests.Helpers
{
    [TestFixture]
    public class ExternalApiAttributeAccessorTests
    {
        private UserApiAttributeAccessor userApiAttributeAccessor;
        private readonly IFixture fixture = new Fixture();
        private Mock<IUserAttributeConfigHelper> userAttributeConfigHelper;
        private Mock<IDataCacheService> dataCacheService;
        private List<UserAttributeConfig> attributes;

        [SetUp]
        public void Setup()
        {
            attributes = fixture.CreateMany<UserAttributeConfig>().ToList();

            dataCacheService = new Mock<IDataCacheService>();

            userAttributeConfigHelper = new Mock<IUserAttributeConfigHelper>();
            userAttributeConfigHelper.Setup(x => x.GetUserAttributeApiConfig(It.IsAny<string>())).ReturnsAsync(new UserAttributeApiConfig
            {
                Attributes = attributes
            });

            userApiAttributeAccessor = new UserApiAttributeAccessor(userAttributeConfigHelper.Object, null, null, dataCacheService.Object);
        }

        [Test]
        public async Task GetDefaultAttributesAsync_ReturnsNames()
        {
            var result = await userApiAttributeAccessor.GetDefaultAttributesAsync();

            result.Should().BeEquivalentTo(attributes.Select(x => x.Name));
        }

        [Test]
        public async Task GetUserAttributesAsync_WhenAttributesNameNull_ReturnsEmptyListOfDetails()
        {
            var result = await userApiAttributeAccessor.GetUserAttributesAsync(fixture.Create<string>(), null);

            result.Should().BeEmpty();
        }

        [Test]
        public async Task GetUserAttributesAsync_WhenAttributesNotNull_ReturnsListOfDetails()
        {
            var jObject = fixture.Create<JObject>();

            dataCacheService.Setup(x => x.GetValue(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Func<string, Task<JObject>>>())).ReturnsAsync(jObject);

            var result = await userApiAttributeAccessor.GetUserAttributesAsync(fixture.Create<string>(), new List<string>());

            dataCacheService.VerifyAll();

            result.Should().BeOfType<List<UserAttributeDetails>>();
        }

        [Test]
        public async Task GetUserAttributesAsync_OnEmptyAttributeValues_ThrowsInvalidResponsePathException()
        {
            var jObject = fixture.Create<JObject>();

            dataCacheService.Setup(x => x.GetValue(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Func<string, Task<JObject>>>())).ReturnsAsync(jObject);

            Func<Task> act = async () => await userApiAttributeAccessor.GetUserAttributesAsync(fixture.Create<string>(), attributes.Select(x => x.Name));

            await act.Should().ThrowAsync<InvalidResponsePathException>();
        }
    }
}
