using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Refinitiv.Aaa.GuissApi.Facade.Helpers;
using Refinitiv.Aaa.GuissApi.Facade.Interfaces;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Refinitiv.Aaa.GuissApi.Facade.Tests.Helpers
{
    [TestFixture]
    public class UserAttributeAccessorHelperTests
    {
        private UserApiAttributeAccessor userApiAttributeAccessor;
        private UserAttributeAccessorHelper userAttributeAccessorHelper;
        private Mock<IUserAttributeConfigHelper> userAttributeConfigHelper;
        private DynamoDbUserAttributeAccessor dynamoDbUserAttributeAccessor;
        private readonly IFixture fixture = new Fixture();
        private List<UserAttributeConfig> attributes;

        [SetUp]
        public void Setup()
        {
            attributes = fixture.CreateMany<UserAttributeConfig>().ToList();

            dynamoDbUserAttributeAccessor = new DynamoDbUserAttributeAccessor(null,null);

            userAttributeConfigHelper = new Mock<IUserAttributeConfigHelper>();
            userAttributeConfigHelper.Setup(x => x.GetUserAttributeApiConfig(It.IsAny<string>())).ReturnsAsync(new UserAttributeApiConfig
            {
                Attributes = attributes
            });

            userApiAttributeAccessor = new UserApiAttributeAccessor(userAttributeConfigHelper.Object, null, null, null);

            userAttributeAccessorHelper = new UserAttributeAccessorHelper(userApiAttributeAccessor, dynamoDbUserAttributeAccessor);
        }

        [Test]
        public async Task GetAccessor_AttributeNamePresentInList_ReturnsAccessor()
        {
            var result = await userAttributeAccessorHelper.GetAccessorAsync(attributes.ElementAt(0).Name);

            result.Should().Be(userApiAttributeAccessor);
        }

        [Test]
        public async Task GetAccessor_AttributeNameNotPresentInList_ReturnsAccessor()
        {
            var result = await userAttributeAccessorHelper.GetAccessorAsync(fixture.Create<string>());

            result.Should().Be(dynamoDbUserAttributeAccessor);
        }

        [Test]
        public async Task GetAccessorsWithAttributes_ReturnsDictionary()
        {
            var result = await userAttributeAccessorHelper.GetAccessorsWithAttributesAsync(attributes.Select(x => x.Name));

            result.Should().BeOfType<Dictionary<IUserAttributeAccessor, List<string>>>();
        }

        [Test]
        public async Task GetAccessorsWithDefaultAttributes_ReturnsDictionary()
        {
            var result = await userAttributeAccessorHelper.GetAccessorsWithDefaultAttributesAsync();

            result.Should().BeOfType<Dictionary<IUserAttributeAccessor, List<string>>>();
        }
    }
}
