﻿using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Refinitiv.Aaa.Foundation.ApiClient.Interfaces;
using Refinitiv.Aaa.GuissApi.Controllers;
using Refinitiv.Aaa.GuissApi.Data.Exceptions;
using Refinitiv.Aaa.GuissApi.Facade.Interfaces;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.UserAttribute;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Refinitiv.Aaa.GuissApi.Tests.Controllers
{
    [TestFixture]
    public class UserAttributeControllerTests
    {
        private UserAttributeController userAttributeController;
        private Mock<IUserAttributeHelper> userAttributeHelper;
        private Mock<IUserAttributeValidator> userAttributeValidator;
        private Mock<ILoggerHelper<UserAttributeController>> loggerHelper;

        [SetUp]
        public void Setup()
        {
            userAttributeHelper = new Mock<IUserAttributeHelper>();
            userAttributeValidator = new Mock<IUserAttributeValidator>();
            loggerHelper = new Mock<ILoggerHelper<UserAttributeController>>();

            userAttributeController = new UserAttributeController(
                userAttributeHelper.Object,
                userAttributeValidator.Object,
                loggerHelper.Object
                );
        }

        [Test]
        public async Task PutReturnsNotFoundObjectResultIfUserDoesntExistInUsersApi()
        {
            var userAttributeDetails = new UserAttributeDetails();

            var expected = new NotFoundObjectResult(new { Message = "The User is not found" });

            userAttributeValidator.Setup(x => x.ValidateAttributeAsync(It.IsAny<UserAttributeDetails>())).ReturnsAsync(expected);

            var result = await userAttributeController.Put(userAttributeDetails);

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Test]
        public async Task PutReturnsSavedItemIfPutValidationIsNull()
        {
            var userAttributeDetails = new UserAttributeDetails();
            var userAttribute = new UserAttribute();

            userAttributeValidator.Setup(x => x.ValidateAttributeAsync(It.IsAny<UserAttributeDetails>())).ReturnsAsync(new AcceptedResult());
            userAttributeValidator.Setup(x => x.ValidatePutRequestAsync(It.IsAny<UserAttributeDetails>())).ReturnsAsync((UserAttribute)null);
            userAttributeHelper.Setup(x => x.InsertAsync(It.IsAny<UserAttributeDetails>())).ReturnsAsync(userAttribute);

            var result = await userAttributeController.Put(userAttributeDetails);

            var okResult = new OkObjectResult(userAttribute);

            result.Should().BeEquivalentTo(okResult);
        }

        [Test]
        public async Task PutReturnsSavedItemIfPutValidationIsNotNull()
        {
            var userAttributeDetails = new UserAttributeDetails();
            var userAttribute = new UserAttribute();

            userAttributeValidator.Setup(x => x.ValidateAttributeAsync(It.IsAny<UserAttributeDetails>())).ReturnsAsync(new AcceptedResult());
            userAttributeValidator.Setup(x => x.ValidatePutRequestAsync(It.IsAny<UserAttributeDetails>())).ReturnsAsync(new UserAttribute());
            userAttributeHelper.Setup(x => x.UpdateAsync(It.IsAny<UserAttribute>(), It.IsAny<string>())).ReturnsAsync(new UserAttribute());

            var result = await userAttributeController.Put(userAttributeDetails);

            var okResult = new OkObjectResult(userAttribute);

            result.Should().BeEquivalentTo(okResult);
        }

        [Test]
        public async Task PutReturnsConflictInCaseOfUpdateConflictException()
        {
            var userAttributeDetails = new UserAttributeDetails();

            userAttributeValidator.Setup(x => x.ValidateAttributeAsync(It.IsAny<UserAttributeDetails>())).ReturnsAsync(new AcceptedResult());
            userAttributeValidator.Setup(x => x.ValidatePutRequestAsync(It.IsAny<UserAttributeDetails>())).ReturnsAsync(new UserAttribute());
            userAttributeHelper.Setup(x => x.UpdateAsync(It.IsAny<UserAttribute>(), It.IsAny<string>())).ThrowsAsync(new UpdateConflictException());

            var result = await userAttributeController.Put(userAttributeDetails);

            result.Should().BeEquivalentTo(new ConflictResult());
        }

    }
}
