using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Refinitiv.Aaa.Foundation.ApiClient.Interfaces;
using Refinitiv.Aaa.Interfaces.Business;
using Refinitiv.Aaa.GuissApi.Controllers;
using Refinitiv.Aaa.GuissApi.Data.Exceptions;
using Refinitiv.Aaa.GuissApi.Data.Models;
using Refinitiv.Aaa.GuissApi.Facade.Exceptions;
using Refinitiv.Aaa.GuissApi.Facade.Interfaces;
using Refinitiv.Aaa.GuissApi.Facade.Models;

namespace Refinitiv.Aaa.GuissApi.Tests.Controllers
{
    [TestFixture]
    public class GuissControllerTests
    {
        private readonly IFixture fixture = new Fixture();
        private readonly Type fixtureType = typeof(GuissController);
        private GuissController controller;
        private Mock<IGuissHelper> templateHelper;

        [SetUp]
        public void Setup()
        {
            templateHelper = new Mock<IGuissHelper>();
            controller = new GuissController(templateHelper.Object, Mock.Of<ILoggerHelper<GuissController>>());
        }

        [Test]
        public async Task GetByIdReturnsGuissFromHelper()
        {
            const string id = "1234";
            var template = fixture.Create<Guiss>();

            templateHelper.Setup(h => h.FindByIdAsync(id))
                .ReturnsAsync(template);

            var result = await controller.Get(id);

            templateHelper.VerifyAll();

            result.Should().BeOfType<JsonResult>("because the template was found")
                .Which.Value
                .Should().BeOfType<Guiss>()
                .And.BeSameAs(template);
        }

        [Test]
        public async Task GetByUuidReturnsGuissFromHelper()
        {
            const string uuid = "Uuid31831203-40a1-478d-a453-48bc9bd99c7e";
            var template = fixture.Create<Guiss>();

            templateHelper.Setup(h => h.FindByIdAsync(uuid))
                .ReturnsAsync(template);

            var result = await controller.Get(uuid);

            templateHelper.VerifyAll();

            result.Should().BeOfType<JsonResult>("because the template was found")
                .Which.Value
                .Should().BeOfType<Guiss>()
                .And.BeSameAs(template);

        }

        [Test]
        public async Task GetByGuissIdReturnsGuissFromHelper()
        {
            const string templateid = "templateid1";
            var template = fixture.Create<Guiss>();

            templateHelper.Setup(h => h.FindByIdAsync(templateid))
                .ReturnsAsync(template);

            var result = await controller.Get(templateid);

            templateHelper.VerifyAll();

            result.Should().BeOfType<JsonResult>("because the template was found")
                .Which.Value
                .Should().BeOfType<Guiss>()
                .And.BeSameAs(template);

        }

        [Test]
        public async Task GetByIdReturnsNotFoundIfGuissDoesNotExist()
        {
            const string id = "1234";
            templateHelper.Setup(h => h.FindByIdAsync(id))
                .ReturnsAsync((Guiss)null);

            var result = await controller.Get(id);

            templateHelper.VerifyAll();

            result.Should().BeOfType<NotFoundResult>("because the template does not exist");
        }

        [Test]
        public async Task PostPassesGuissDetailsToTheHelper()
        {
            Guiss templatePassedToHelper = null;

            templateHelper.Setup(h => h.InsertAsync(It.IsAny<Guiss>()))
                .Callback<Guiss>(u => templatePassedToHelper = u)
                .ReturnsAsync(new Guiss());

            var submittedDetails = fixture.Create<GuissDetails>();

            await controller.Post(submittedDetails);

            templateHelper.VerifyAll();

            using (new AssertionScope())
            {
                templatePassedToHelper.Should().NotBeNull("because the template details should be passed to the helper")
                    .And.BeEquivalentTo(submittedDetails);
            }
        }

        [Test]
        public async Task PostReturnsTheCreatedGuiss()
        {
            var createdGuiss = fixture.Create<Guiss>();

            templateHelper.Setup(h => h.InsertAsync(It.IsAny<Guiss>()))
                .ReturnsAsync(createdGuiss);

            var result = await controller.Post(fixture.Create<GuissDetails>());

            templateHelper.VerifyAll();

            result.Should().BeOfType<CreatedAtActionResult>()
                .Which.Value
                .Should().BeOfType<Guiss>()
                .And.BeSameAs(createdGuiss);
        }

        [Test]
        public void PostNullThrowsArgumentNullException()
        {
            controller.Awaiting(c => c.Post(null))
                .Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("details");
        }

        [Test]
        public async Task PutReturnsTheUpdatedGuiss()
        {
            var submittedGuiss = fixture.Create<Guiss>();
            var updatedGuiss = fixture.Create<Guiss>();

            templateHelper.Setup(h => h.UpdateAsync(submittedGuiss))
                .ReturnsAsync(updatedGuiss);

            templateHelper.Setup(h => h.FindByIdAsync(submittedGuiss.Id))
                .ReturnsAsync(new Guiss
                {
                    Id = submittedGuiss.Id,
                    Name = submittedGuiss.Name
                });

            var result = await controller.Put(submittedGuiss.Id, submittedGuiss);

            templateHelper.VerifyAll();

            result.Should().BeOfType<OkObjectResult>()
                .Which.Value
                .Should().BeOfType<Guiss>()
                .And.BeSameAs(updatedGuiss);
        }

        [Test]
        public async Task PutReturnsNotFoundIfGuissDoesNotAlreadyExist()
        {
            var submittedGuiss = fixture.Create<Guiss>();

            templateHelper.Setup(h => h.FindByIdAsync(submittedGuiss.Id))
                .ReturnsAsync((Guiss)null);

            var result = await controller.Put(submittedGuiss.Id, submittedGuiss);

            templateHelper.VerifyAll();

            result.Should().BeOfType<NotFoundResult>("because the template does not exist");
        }

        [Test]
        public async Task PutReturnsConflictIfUpdateConflictExceptionIsThrown()
        {
            var submittedGuiss = fixture.Create<Guiss>();

            templateHelper.Setup(h => h.FindByIdAsync(submittedGuiss.Id))
                .ReturnsAsync(new Guiss
                {
                   Id = submittedGuiss.Id,
                   Name = submittedGuiss.Name
                });

            templateHelper.Setup(h => h.UpdateAsync(submittedGuiss))
                .ThrowsAsync(new UpdateConflictException());

            var result = await controller.Put(submittedGuiss.Id, submittedGuiss);

            templateHelper.VerifyAll();

            result.Should().BeOfType<ConflictResult>("because an Update Conflict exception occurred");
        }

        [Test]
        public async Task PutReturnsPreconditionFailedIfIdDoesNotMatchUrl()
        {
            var result = await controller.Put("123", new Guiss { Id = "456" });
            result.Should().BeOfType<StatusCodeResult>("because the IDs in the URL and body do not match")
                .Which.StatusCode.Should().Be((int)HttpStatusCode.PreconditionFailed);
        }



        [Test]
        public async Task DeleteReturnsNoContent()
        {
            const string id = "123";
            templateHelper.Setup(h => h.FindByIdAsync(id))
                .ReturnsAsync(new Guiss());

            var result = await controller.Delete(id);

            result.Should().BeOfType<NoContentResult>();
        }

        [Test]
        public async Task DeleteCallsDeleteOnTheGuissHelper()
        {
            const string id = "123";
            templateHelper.Setup(h => h.FindByIdAsync(id))
                .ReturnsAsync(new Guiss());

            templateHelper.Setup(h => h.DeleteAsync(id));

            await controller.Delete(id);

            templateHelper.VerifyAll();
        }

        [Test]
        public async Task DeleteReturnsNotFoundIfGuissDoesNotExist()
        {
            var result = await controller.Delete("Non-existent ID");
            result.Should().BeOfType<NotFoundResult>("because the template does not exist");
        }

        [Test]
        public void GetAllHasTheCorrectRESTMethod()
        {
            var method = fixtureType.GetMethod("Get", new[] { typeof(GuissFilter), typeof(int?), typeof(string) });
            Assert.That(method, Has.Attribute<HttpGetAttribute>());
        }

        [Test]
        public async Task GetAllReturnsAGuissResultSet()
        {
            GuissResultSet templateObject = new GuissResultSet();
            templateObject.Items = new List<Guiss>
            {
                new Guiss {Id = "2"}
            };

            templateHelper.Setup(g => g.FindAllAsync((GuissFilter)null, It.IsAny<Facade.Models.PaginationModel>())).ReturnsAsync(templateObject);

            var result = await controller.Get();

            result.Should().BeOfType<JsonResult>("because a result is always returned")
                .Which.Value
                .Should().BeOfType<GuissResultSet>()
                .And.BeSameAs(templateObject);
        }

        [Test]
        public async Task GetAllReturnsBadRequestIfPaginationTokenIsInvalid()
        {
            templateHelper.Setup(x => x.FindAllAsync(It.IsAny<GuissFilter>(), It.IsAny<IPagingModel>()))
                .ThrowsAsync(new InvalidPaginationTokenException());

            var result = await controller.Get(pagination: "Invalid");

            result.Should().BeOfType<BadRequestResult>("because the pagination token is invalid");
        }

        [TestCase(0)]
        [TestCase(-1)]
        public async Task GetAllReturnsBadRequestIfLimitIsLessThan1(int limit)
        {
            var result = await controller.Get(limit: limit);
            result.Should().BeOfType<BadRequestResult>("because the paging limit is out of range");
        }

        [Test]
        public async Task GetAllReturnsNextAndPreviousTokens()
        {
            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper.Setup(url => url.Link(It.IsAny<string>(), It.IsAny<object>()))
                .Returns((string routeName, object values) => {
                    var token = values?.GetType()?.GetProperty("pagination")?.GetValue(values) as string;
                    return $"https://template-api.com/api/Guiss?paginationToken={token ?? string.Empty}";
                });

            controller.Url = mockUrlHelper.Object;

            var templates = new GuissResultSet { Next = "next", Previous = "previous" };

            templateHelper.Setup(u => u.FindAllAsync(It.IsAny<GuissFilter>(), It.IsAny<IPagingModel>()))
                .ReturnsAsync(templates);

            var result = (JsonResult)await controller.Get();

            var returnValue = (GuissResultSet)result.Value;

            using (new AssertionScope())
            {
                returnValue.Next.Should().Be("next");
                returnValue.Previous.Should().Be("previous");
            }
        }

        [Test]
        public async Task GetAllPassesPaginationSettingsToGuissHelper()
        {
            templateHelper.Setup(u => u.FindAllAsync(It.IsAny<GuissFilter>(), It.IsAny<IPagingModel>()))
                .ReturnsAsync(new GuissResultSet());

            var filter = fixture.Create<GuissFilter>();

            await controller.Get(filter, 2, "token");

            templateHelper.Verify(h => h.FindAllAsync(filter, It.Is<IPagingModel>(p => p.Limit == 2 && p.Pagination == "token")));
        }
    }
}
