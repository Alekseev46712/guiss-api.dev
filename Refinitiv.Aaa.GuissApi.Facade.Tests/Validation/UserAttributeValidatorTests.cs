using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Refinitiv.Aaa.Foundation.ApiClient.Core.Models.User;
using Refinitiv.Aaa.Foundation.ApiClient.Interfaces;
using Refinitiv.Aaa.GuissApi.Data.Interfaces;
using Refinitiv.Aaa.GuissApi.Data.Models;
using Refinitiv.Aaa.GuissApi.Facade.Mapping;
using Refinitiv.Aaa.GuissApi.Facade.Validation;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.UserAttribute;
using System;
using System.Threading.Tasks;

namespace Refinitiv.Aaa.GuissApi.Facade.Tests.Validation
{
    [TestFixture]
    public class UserAttributeValidatorTests
    {
        private Mock<IUserAttributeRepository> userAttributeRepository;
        private Mock<IUserHelper> userHelper;
        private UserAttributeValidator userAttributeValidator;
        private IMapper mapper;


        [SetUp]
        public void Setup()
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new UserAttributeMappingProfile());
            });

            mapper = mappingConfig.CreateMapper();
            userAttributeRepository = new Mock<IUserAttributeRepository>();
            userHelper = new Mock<IUserHelper>();

            userAttributeValidator = new UserAttributeValidator(
                userHelper.Object,
                userAttributeRepository.Object,
                mapper
                );
        }

        [Test]
        public async Task ValidateUserUuidAsyncReturnsNotFoundObjectResultIfUserDoesNotExist()
        {
            var uuid = "test";
            userHelper.Setup(h => h.GetUserByUuidAsync(It.IsAny<string>())).ReturnsAsync((UserResponse) null);
            var result = await userAttributeValidator.ValidateUserUuidAsync(uuid);
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Test]
        public async Task ValidateUserUuidAsyncReturnsAcceptedResultIfUserExists()
        {
            var uuid = "test";
            var userResponse = new UserResponse { Uuid = uuid };
            
            userHelper.Setup(h => h.GetUserByUuidAsync(It.IsAny<string>())).ReturnsAsync(userResponse);
            var result = await userAttributeValidator.ValidateUserUuidAsync(uuid);
            result.Should().BeOfType<AcceptedResult>();
        }

        [Test]
        public async Task ValidateUserUuidAsyncReturnsBadRequestObjectResultIfUuidIsNull()
        {
            var result = await userAttributeValidator.ValidateUserUuidAsync(null);
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public void ValidateAttributeAsyncThrowsExceptionIfArgumentIsNull()
        {
            UserAttributeDetails userAttributeDetails = null;

            Assert.ThrowsAsync<ArgumentNullException>(() => userAttributeValidator.ValidateAttributeAsync(userAttributeDetails));
        }

        [Test]
        public void ValidatePutRequestAsyncThrowsExceptionIfArgumentIsNull()
        {
            UserAttributeDetails userAttributeDetails = null;

            Assert.ThrowsAsync<ArgumentNullException>(() => userAttributeValidator.ValidatePutRequestAsync(userAttributeDetails));
        }

        [Test]
        public async Task ValidateAttributeAsyncReturnsNotFoundObjectResultIfUserDoesntExistInUsersApi()
        {
            userHelper.Setup(x => x.GetUserByUuidAsync(It.IsAny<string>())).ReturnsAsync((UserResponse)null);

            var userAttributeDetails = new UserAttributeDetails() { UserUuid = "UUID of non existing user" };

            var result = await userAttributeValidator.ValidateAttributeAsync(userAttributeDetails);

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Test]
        public async Task ValidatePutRequestAsyncReturnsNullIfAttributeDoesntExistInDb()
        {
            userAttributeRepository.Setup(x => x.FindByUserUuidAndNameAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((UserAttributeDb)null);

            var userAttributeDetails = new UserAttributeDetails() { UserUuid = "UUID of non existing attribute", Name = "Name of non existing attribute" };

            var result = await userAttributeValidator.ValidatePutRequestAsync(userAttributeDetails);

            result.Should().BeNull();
        }

        [Test]
        public async Task ValidateAttributeAsyncReturnsAcceptedResultIfUserExist()
        {
            userHelper.Setup(x => x.GetUserByUuidAsync(It.IsAny<string>())).ReturnsAsync(new UserResponse());

            var userAttributeDetails = new UserAttributeDetails() { UserUuid = "UUID of existing user" };

            var result = await userAttributeValidator.ValidateAttributeAsync(userAttributeDetails);

            result.Should().BeOfType<AcceptedResult>();
        }

        [Test]
        public async Task ValidatePutRequestAsyncReturnsUserAttributeIfItExistInDb()
        {
            var userAttributeDetails = new UserAttributeDetails() { UserUuid = "UUID of existing attribute", Name = "Name of existing attribute" };

            userAttributeRepository.Setup(x => x.FindByUserUuidAndNameAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new UserAttributeDb() { UserUuid = "UUID of existing attribute", Name = "Name of existing attribute" });

            var expected = new UserAttribute() { UserUuid = "UUID of existing attribute", Name = "Name of existing attribute" };

            var result = await userAttributeValidator.ValidatePutRequestAsync(userAttributeDetails);

            result.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task ValidateUserAttributesAsync_WhenUserUuidOrNameIsNull_ReturnNotFoundObjectResult()
        {
            userAttributeRepository.Setup(x => x.FindByUserUuidAndNameAsync(It.IsAny<string>(), It.IsAny<string>()));

            var result = await userAttributeValidator.ValidateUserAttributesAsync(null, null);

            userAttributeRepository.VerifyAll();

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Test]
        public async Task ValidateUserAttributesAsync_WhenUserAttribiteNotFound_ReturnNotFoundObjectResult()
        {
            var userUuid = "testUserUuid";
            var name = "testName";

            userAttributeRepository.Setup(x => x.FindByUserUuidAndNameAsync(userUuid, name));

            var result = await userAttributeValidator.ValidateUserAttributesAsync(userUuid, name);

            userAttributeRepository.VerifyAll();

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Test]
        public async Task ValidateUserAttributesAsync_WhenUserAttribiteFound_ReturnAcceptedResult()
        {
            var userUuid = "testUserUuid";
            var name = "testName";

            userAttributeRepository.Setup(x => x.FindByUserUuidAndNameAsync(userUuid, name)).ReturnsAsync(new UserAttributeDb());

            var result = await userAttributeValidator.ValidateUserAttributesAsync(userUuid, name);

            userAttributeRepository.VerifyAll();

            result.Should().BeOfType<AcceptedResult>();
        }

        [Test]
        public void ValidateNamespacesString_WhenNamespacesAreNull_ReturnBadRequestObjectResult()
        {
            var result = userAttributeValidator.ValidateNamespacesString(null);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public void ValidateNamespacesString_WhenNamespacesContainDot_ReturnBadRequestObjectResult()
        {
            var invalidNamespaces = "Level1.level2";

            var result = userAttributeValidator.ValidateNamespacesString(invalidNamespaces);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public void ValidateNamespacesString_WhenValidationPassed_ReturnAcceptedResult()
        {
            var validNamespace = "Level1";

            var result = userAttributeValidator.ValidateNamespacesString(validNamespace);

            result.Should().BeOfType<AcceptedResult>();
        }
    }
}
