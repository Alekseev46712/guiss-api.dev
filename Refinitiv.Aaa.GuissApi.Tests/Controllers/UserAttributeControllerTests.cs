using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Refinitiv.Aaa.Foundation.ApiClient.Interfaces;
using Refinitiv.Aaa.GuissApi.Controllers;
using Refinitiv.Aaa.GuissApi.Data.Exceptions;
using Refinitiv.Aaa.GuissApi.Facade.Interfaces;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.UserAttribute;
using System.Threading.Tasks;
using AutoFixture;
using Newtonsoft.Json.Linq;

namespace Refinitiv.Aaa.GuissApi.Tests.Controllers
{
    [TestFixture]
    public class UserAttributeControllerTests
    {
        private readonly IFixture fixture = new Fixture();
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

        #region Get

        [Test]
        public async Task Get_ShouldCallGetAllByUserUuidAsyncAndReturnJObject()
        {
            var jsonData = fixture.Create<JObject>();
            var userUuid = fixture.Create<string>();

            userAttributeHelper.Setup(g => g.GetAllByUserUuidAsync(userUuid))
                .ReturnsAsync(jsonData);

            var result = await userAttributeController.Get(userUuid);

            userAttributeHelper.VerifyAll();

            result.Should().BeOfType<OkObjectResult>("because a result is always returned")
                .Which.Value
                .Should().BeEquivalentTo(jsonData);
        }

        [Test]
        public async Task GetUserAttributes_ShouldCallGetAttributesByUserUuidAsyncAndReturnJObject()
        {
            var jsonData = fixture.Create<JObject>();
            var userUuid = fixture.Create<string>();
            var attributes = fixture.Create<string>();

            userAttributeValidator.Setup(u => u.ValidateUserUuidAsync(userUuid)).ReturnsAsync(new AcceptedResult());
            userAttributeValidator.Setup(u => u.ValidateAttributesString(attributes)).Returns(new AcceptedResult());

            userAttributeHelper.Setup(g => g.GetAttributesByUserUuidAsync(userUuid, attributes))
                .ReturnsAsync(jsonData);

            var result = await userAttributeController.Get(userUuid, attributes);

            userAttributeHelper.VerifyAll();

            result.Should().BeOfType<OkObjectResult>("because a result is always returned")
                .Which.Value
                .Should().BeEquivalentTo(jsonData);
        }

        [Test]
        public async Task GetUserAttributes_OnUserNotFound_ShouldReturnNotFound()
        {
            var userUuid = fixture.Create<string>();
            var attributes = fixture.Create<string>();

            userAttributeValidator.Setup(u => u.ValidateUserUuidAsync(userUuid)).ReturnsAsync(new NotFoundObjectResult(new { Message = "The User is not found" }));

            var result = await userAttributeController.Get(userUuid, attributes);

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Test]
        public async Task GetUserAttributes_OnNoAttributes_ShouldReturnBadRequest()
        {
            var userUuid = fixture.Create<string>();
            string attributes = ",";

            userAttributeValidator.Setup(u => u.ValidateUserUuidAsync(userUuid)).ReturnsAsync(new AcceptedResult());
            userAttributeValidator.Setup(u => u.ValidateAttributesString(attributes)).Returns(new BadRequestObjectResult("test"));

            var result = await userAttributeController.Get(userUuid, attributes);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task GetByNamespaces_OnUserNotFound_ShouldReturnNotFound()
        {
            var userUuid = fixture.Create<string>();
            var attributes = fixture.Create<string>();

            userAttributeValidator.Setup(u => u.ValidateUserUuidAsync(userUuid)).ReturnsAsync(new NotFoundObjectResult(new { Message = "The User is not found" }));

            var result = await userAttributeController.GetByNamespaces(userUuid, attributes);

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Test]
        public async Task GetByNamespaces_OnNoNamespaces_ShouldReturnBadRequest()
        {
            var userUuid = fixture.Create<string>();
            string namespaces = ",";

            userAttributeValidator.Setup(u => u.ValidateUserUuidAsync(userUuid)).ReturnsAsync(new AcceptedResult());
            userAttributeValidator.Setup(u => u.ValidateNamespacesString(namespaces)).Returns(new BadRequestObjectResult("test"));

            var result = await userAttributeController.GetByNamespaces(userUuid, namespaces);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task GetByNamespaces_ShouldCallGetAttributesByNamespacesAndUuidAsyncAndReturnJObject()
        {
            var jsonData = fixture.Create<JObject>();
            var userUuid = fixture.Create<string>();
            var namespaces = fixture.Create<string>();

            userAttributeValidator.Setup(u => u.ValidateUserUuidAsync(userUuid)).ReturnsAsync(new AcceptedResult());
            userAttributeValidator.Setup(u => u.ValidateNamespacesString(namespaces)).Returns(new AcceptedResult());

            userAttributeHelper.Setup(g => g.GetAttributesByUserNamespacesAndUuidAsync(userUuid, namespaces))
                .ReturnsAsync(jsonData);

            var result = await userAttributeController.GetByNamespaces(userUuid, namespaces);
            userAttributeHelper.VerifyAll();

            result.Should().BeOfType<OkObjectResult>("because a result is always returned")
                .Which.Value
                .Should().BeEquivalentTo(jsonData);
        }

        #endregion

        #region Put

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

        #endregion

        #region Delete

        [Test]
        public async Task DeleteUserAttribute_WhenUserAttributeNotFound_ReturnsNotFoundObjectResult()
        {
            var userUuid = fixture.Create<string>();
            var name = fixture.Create<string>();

            userAttributeValidator.Setup(x => x.ValidateUserAttributesAsync(userUuid, name)).ReturnsAsync(new NotFoundObjectResult(new { Message = "The User is not found" }));

            var result = await userAttributeController.DeleteUserAttribute(userUuid, name);

            userAttributeValidator.VerifyAll();

            result.Should().BeOfType<NotFoundObjectResult>("Because UserAttribitte in this test always not found");
        }

        [Test]
        public async Task DeleteUserAttribute_WhenValidationSuccess_ReturnStatus204NoContent()
        {
            var userUuid = fixture.Create<string>();
            var name = fixture.Create<string>();

            userAttributeValidator.Setup(x => x.ValidateUserAttributesAsync(userUuid, name)).ReturnsAsync(new AcceptedResult());
            userAttributeHelper.Setup(x => x.DeleteUserAttributeAsync(userUuid, name));

            var result = await userAttributeController.DeleteUserAttribute(userUuid, name);

            userAttributeValidator.VerifyAll();
            userAttributeHelper.VerifyAll();

            result.Should().BeOfType<NoContentResult> ("Because UserAttribitte succsessfully deleted");
        }

        #endregion
    }
}
