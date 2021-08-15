using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Refinitiv.Aaa.GuissApi.Facade.Helpers;
using Refinitiv.Aaa.GuissApi.Facade.Interfaces;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.Configuration;
using System.Collections.Generic;
using System.Linq;

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
            userAttributeConfigHelper.Setup(x => x.GetUserAttributeApiConfig(It.IsAny<string>())).Returns(new UserAttributeApiConfig
            {
                Attributes = attributes
            });

            userApiAttributeAccessor = new UserApiAttributeAccessor(userAttributeConfigHelper.Object, null, null, null);

            userAttributeAccessorHelper = new UserAttributeAccessorHelper(userApiAttributeAccessor, dynamoDbUserAttributeAccessor);
        }

        [Test]
        public void GetAccessor_AttributeNamePresentInList_ReturnsAccessor()
        {
            var result = userAttributeAccessorHelper.GetAccessor(attributes.ElementAt(0).Name);

            result.Should().Be(userApiAttributeAccessor);
        }

        [Test]
        public void GetAccessor_AttributeNameNotPresentInList_ReturnsAccessor()
        {
            var result = userAttributeAccessorHelper.GetAccessor(fixture.Create<string>());

            result.Should().Be(dynamoDbUserAttributeAccessor);
        }

        [Test]
        public void GetAccessorsWithAttributes_ReturnsDictionary()
        {
            var result = userAttributeAccessorHelper.GetAccessorsWithAttributes(attributes.Select(x => x.Name));

            result.Should().BeOfType<Dictionary<IUserAttributeAccessor, List<string>>>();
        }

        [Test]
        public void GetAccessorsWithDefaultAttributes_ReturnsDictionary()
        {
            var result = userAttributeAccessorHelper.GetAccessorsWithDefaultAttributes();

            result.Should().BeOfType<Dictionary<IUserAttributeAccessor, List<string>>>();
        }
    }
}
