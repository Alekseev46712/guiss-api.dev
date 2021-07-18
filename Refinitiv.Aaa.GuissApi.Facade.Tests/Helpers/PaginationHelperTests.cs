using System;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Refinitiv.Aaa.Interfaces.Business;
using Refinitiv.Aaa.Pagination.Interfaces;
using Refinitiv.Aaa.Pagination.Models;
using Refinitiv.Aaa.GuissApi.Data.Models;
using Refinitiv.Aaa.GuissApi.Facade.Exceptions;
using Refinitiv.Aaa.GuissApi.Facade.Helpers;
using Refinitiv.Aaa.GuissApi.Facade.Interfaces;
using Refinitiv.Aaa.GuissApi.Facade.Models;

namespace Refinitiv.Aaa.GuissApi.Facade.Tests.Helpers
{
    [TestFixture]
    public class PaginationHelperTests
    {
        private Mock<IAppSettingsConfiguration> appSettingConfiguration;
        private Mock<IPaginationService> paginationService;
        private IPaginationHelper paginationHelper;

        [SetUp]
        public void Setup()
        {
            appSettingConfiguration = new Mock<IAppSettingsConfiguration>();
            appSettingConfiguration.SetupGet(x => x.DefaultQueryLimit).Returns(2);

            paginationService = new Mock<IPaginationService>();
            paginationService.Setup(x => x.CreatePageToken(It.IsAny<Cursor<GuissFilter>>())).Returns("paginationToken");

            paginationHelper = new PaginationHelper(appSettingConfiguration.Object, paginationService.Object);
        }

        [Test]
        public void SetupCursor_Throws_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => paginationHelper.SetupCursor<IModel>(null, null));
        }

        [Test]
        public void SetupCursor_Returns_Default_Cursor_When_No_PaginationToken_IsPresent_And_Sets_Previous()
        {
            var templateDetails = new GuissResultSet();
            var result = paginationHelper.SetupCursor(null, templateDetails);

            result.Should().BeEquivalentTo(new Cursor<GuissFilter>(2, null));
            templateDetails.Previous.Should().NotBeNull();
        }

        [Test]
        public void SetupCursor_Returns_Cursor_When_PaginationToken_IsPresent_WithoutLastEvaluatedKey()
        {
            paginationService.Setup(x => x.GetPageObject<GuissFilter>(It.IsAny<string>()))
                .Returns(() => new Cursor<GuissFilter>(2, new GuissFilter{ Kind = "TEMPLATE" }, null));

            var result = paginationHelper.SetupCursor(null, new GuissResultSet(), new PaginationModel
            {
                Pagination = Guid.NewGuid().ToString()
            });

            result.Should().BeEquivalentTo(new Cursor<GuissFilter>(2, new GuissFilter{ Kind = "TEMPLATE" }));
        }

        [Test]
        public void SetupCursor_Returns_Cursor_And_Sets_Previous_When_PaginationToken_IsPresent_WithLastEvaluatedKey()
        {
            paginationService.Setup(x => x.GetPageObject<GuissFilter>(It.IsAny<string>()))
                .Returns(() => new Cursor<GuissFilter>(2, new GuissFilter() { Kind = "TEMPLATE" }, "{\"Kind\":{\"S\":\"TEMPLATE\"},\"Id\":{\"S\":\"b47022b5-070c-4686-b136-94aa5fa651de\"}}"));

            var templateDetails = new GuissResultSet();
            var result = paginationHelper.SetupCursor(new GuissFilter(), templateDetails, new PaginationModel
            {
                Pagination = Guid.NewGuid().ToString()
            });

            result.Should().BeEquivalentTo(new Cursor<GuissFilter>(2, new GuissFilter() { Kind = "TEMPLATE" }, "{\"Kind\":{\"S\":\"TEMPLATE\"},\"Id\":{\"S\":\"b47022b5-070c-4686-b136-94aa5fa651de\"}}"));
            templateDetails.Previous.Should().NotBeNullOrEmpty();
            templateDetails.Next.Should().BeNullOrEmpty();
        }

        [Test]
        public void SetupCursor_Returns_Cursor_And_Sets_Next_When_PaginationToken_IsPresent_WithLastEvaluatedKey()
        {
            paginationService.Setup(x => x.GetPageObject<GuissFilter>(It.IsAny<string>()))
                .Returns(() => new Cursor<GuissFilter>(2, new GuissFilter(){Kind="TEMPLATE"}, "lastEvaluatedString", true));

            var templateDetails = new GuissResultSet();
            var result = paginationHelper.SetupCursor(new GuissFilter(), templateDetails, new PaginationModel
            {
                Pagination = Guid.NewGuid().ToString()
            });

            result.Should().BeEquivalentTo(new Cursor<GuissFilter>(2, new GuissFilter() { Kind = "TEMPLATE" }, "lastEvaluatedString", true));
            templateDetails.Previous.Should().BeNullOrEmpty();
            templateDetails.Next.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void CreatePaginationToken_Returns_TokenString()
        {
            string result1 = "{\"Kind\":{\"S\":\"TEMPLATE\"},\"Id\":{\"S\":\"b\"}}";
            var result = paginationHelper.CreatePaginationToken(new Cursor<GuissFilter>(1, new GuissFilter(), result1));
            result.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void CreatePaginationToken_Returns_Null_When_CreatePageToken_Returns_EmptyString()
        {
            paginationService.Setup(x => x.CreatePageToken(It.IsAny<Cursor<GuissFilter>>()))
                .Returns(() => null);

            var result = paginationHelper.CreatePaginationToken(new Cursor<GuissFilter>(50, null));
            result.Should().BeNull();
        }

        [Test]
        public void SetupCursor_Uses_PagingModel_Limit()
        {
            paginationService.Setup(x => x.GetPageObject<GuissFilter>(It.IsAny<string>()))
                .Returns(() => new Cursor<GuissFilter>(10, new GuissFilter() { Kind = "TEMPLATE" }, "lastEvaluatedString", true));
            
            var templateDetails = new GuissResultSet();
            var result = paginationHelper.SetupCursor(null, templateDetails, new PaginationModel
            {
                Pagination = Guid.NewGuid().ToString(),
                Limit = 10
            });

            result.Limit.Should().Be(10);
        }

        [Test]
        public void SetupCursor_Throws_InvalidPaginationTokenException()
        {
            paginationService.Setup(x => x.GetPageObject<GuissFilter>(It.IsAny<string>()))
                .Throws<FormatException>();

            Assert.Throws<InvalidPaginationTokenException>(() =>
            {
                paginationHelper.SetupCursor(null, new GuissResultSet(), new PaginationModel
                {
                    Pagination = Guid.NewGuid().ToString(),
                    Limit = 10
                });
            });
        }
    }
}
