using AutoFixture;
using FluentAssertions;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Refinitiv.Aaa.Foundation.ApiClient.Exceptions;
using Refinitiv.Aaa.Foundation.ApiClient.Interfaces;
using Refinitiv.Aaa.GuissApi.Facade.Exceptions;
using Refinitiv.Aaa.GuissApi.Facade.Helpers;
using Refinitiv.Aaa.GuissApi.Facade.Interfaces;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.Configuration;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.UserAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        List<UserAttributeConfig> attributes;


        [SetUp]
        public void Setup()
        {

            attributes = fixture.CreateMany<UserAttributeConfig>().ToList();

            dataCacheService = new Mock<IDataCacheService>();

            userAttributeConfigHelper = new Mock<IUserAttributeConfigHelper>();
            userAttributeConfigHelper.Setup(x => x.GetUserAttributeApiConfig(It.IsAny<string>())).Returns(new UserAttributeApiConfig
            {
                Attributes = attributes
            });

            userApiAttributeAccessor = new UserApiAttributeAccessor(userAttributeConfigHelper.Object, null, null, dataCacheService.Object);
        }

        [Test]
        public void DefaultAttributesReturnsNames()
        {        
            var result = userApiAttributeAccessor.DefaultAttributes;

            result.Should().BeEquivalentTo(attributes.Select(x => x.Name));
        }

        [Test]
        public async Task GetUserAttributesAsyncReturnsEmptyListOfDetailsWhenAttributesNameNull()
        {
            var result = await userApiAttributeAccessor.GetUserAttributesAsync(fixture.Create<string>(), null);

            result.Should().BeEmpty();
        }

        [Test]
        public async Task GetUserAttributesAsyncReturnsListWhenAttributesNotNull()
        {
            var JObj = fixture.Create<JObject>();

            dataCacheService.Setup(x => x.GetValue(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Func<string, Task<JObject>>>())).ReturnsAsync(JObj);

            var result = await userApiAttributeAccessor.GetUserAttributesAsync(fixture.Create<string>(), new List<string>());

            dataCacheService.VerifyAll();

            result.Should().BeOfType<List<UserAttributeDetails>>();

        }

        [Test]
        public async Task GetUserAttributesAsyncThrowsInvalidResponsePathExceptionOnEmptyAttributeValues()
        {
            var JObj = fixture.Create<JObject>();

            dataCacheService.Setup(x => x.GetValue(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Func<string, Task<JObject>>>())).ReturnsAsync(JObj);

            Func<Task> act = async () => await userApiAttributeAccessor.GetUserAttributesAsync(fixture.Create<string>(), attributes.Select(x => x.Name));

            await act.Should().ThrowAsync<InvalidResponsePathException>();

        }
    }
}
