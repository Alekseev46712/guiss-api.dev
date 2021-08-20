using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Refinitiv.Aaa.GuissApi.Facade.Helpers;
using Refinitiv.Aaa.GuissApi.Facade.Interfaces;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.UserAttribute;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Refinitiv.Aaa.GuissApi.Facade.Tests.Helpers
{
    [TestFixture]
    public class UserAttributeProviderTests
    {
        private Mock<IUserAttributeAccessorHelper> accessorsHelper;
        private UserAttributeProvider userAttributeProvider;
        private readonly IFixture fixture = new Fixture();

        [SetUp]
        public void Setup()
        {
            accessorsHelper = new Mock<IUserAttributeAccessorHelper>();

            userAttributeProvider = new UserAttributeProvider(accessorsHelper.Object);
        }

        [Test]
        public async Task GetUserAttributesAsyncWithStringParam_ReturnsListOfDetails()
        {
            accessorsHelper.Setup(x => x.GetAccessorsWithDefaultAttributesAsync()).ReturnsAsync(new Dictionary<IUserAttributeAccessor, List<string>>());

            var result = await userAttributeProvider.GetUserAttributesAsync(fixture.Create<string>());

            result.Should().BeOfType<List<UserAttributeDetails>>();
        }

        [Test]
        public async Task GetUserAttributeAsyncWithStringAndListParams_ReturnsListOfDetails()
        {
            var testAccessor = new Mock<IUserAttributeAccessor>();
            var dict = new Dictionary<IUserAttributeAccessor, List<string>>();

            testAccessor.Setup(x => x
                    .GetUserAttributesAsync(It.IsAny<string>(), It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(new List<UserAttributeDetails>());
            dict.Add(testAccessor.Object, new List<string>());
            accessorsHelper.Setup(x => x
                    .GetAccessorsWithAttributesAsync(It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(dict);

            var result = await userAttributeProvider.GetUserAttributesAsync(fixture.Create<string>(), fixture.CreateMany<string>());

            result.Should().BeOfType<List<UserAttributeDetails>>();
        }
    }
}
