using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Refinitiv.Aaa.Foundation.ApiClient.Core.Models.User;
using Refinitiv.Aaa.Foundation.ApiClient.Interfaces;
using Refinitiv.Aaa.GuissApi.Data.Interfaces;
using Refinitiv.Aaa.GuissApi.Data.Models;
using Refinitiv.Aaa.GuissApi.Facade.Helpers;
using Refinitiv.Aaa.GuissApi.Facade.Mapping;
using Refinitiv.Aaa.GuissApi.Facade.Validation;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.UserAttribute;
using Refinitiv.Aaa.Interfaces.Headers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Refinitiv.Aaa.GuissApi.Facade.Tests.Validation
{
    [TestFixture]
    class UserAttributeValidatorTest
    {
        private Mock<IUserAttributeRepository> userAttributeRepository;
        private Mock<IAaaRequestHeaders> aaaRequestHeaders;
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

            aaaRequestHeaders = new Mock<IAaaRequestHeaders>();

            userAttributeRepository = new Mock<IUserAttributeRepository>();

            userHelper = new Mock<IUserHelper>();

            userAttributeValidator = new UserAttributeValidator(
                userHelper.Object,
                userAttributeRepository.Object,
                mapper
                );
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
        public async Task ValidateAttributeAsyncReturnsNotFoundObjectResultIfUserDoesntExistInUsersApu()
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

    }
}
