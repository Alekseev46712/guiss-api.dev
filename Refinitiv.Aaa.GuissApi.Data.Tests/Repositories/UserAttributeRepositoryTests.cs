using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Refinitiv.Aaa.GuissApi.Data.Exceptions;
using Refinitiv.Aaa.GuissApi.Data.Interfaces;
using Refinitiv.Aaa.GuissApi.Data.Models;
using Refinitiv.Aaa.GuissApi.Data.Repositories;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.Configuration;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.UserAttribute;
using Refinitiv.Aaa.Pagination.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Refinitiv.Aaa.GuissApi.Data.Tests.Repositories
{
    [TestFixture]
    public class UserAttributeRepositoryTests
    {
        private const string TableName = "UserAtributeTable";

        private UserAttributeRepository repository;
        private Mock<IDynamoDBContext> dynamoDbContext;
        private Mock<ILogger<UserAttributeRepository>> mockLogger;
        private Mock<IDynamoDbDocumentQueryWrapper<UserAttributeDb, UserAttributeFilter>> mockQueryWrapper;
        private IFixture fixture;

        [SetUp]
        public void SetUp()
        {
            fixture = new Fixture();
            mockLogger = new Mock<ILogger<UserAttributeRepository>>();
            dynamoDbContext = new Mock<IDynamoDBContext>();
            mockQueryWrapper = new Mock<IDynamoDbDocumentQueryWrapper<UserAttributeDb, UserAttributeFilter>>();
            var appSettings = new AppSettings { DynamoDb = new DynamoDbConfiguration { UserAttributeTableName = TableName } };
            var appSettingsMock = new Mock<IOptions<AppSettings>>();
            appSettingsMock.Setup(ap => ap.Value).Returns(appSettings);
            repository = new UserAttributeRepository(dynamoDbContext.Object, appSettingsMock.Object, mockQueryWrapper.Object, mockLogger.Object);
        }

        #region FindByUserUuidAndName

        [Test]
        public async Task FindByUserUuidAndNameAsync_WhenUserAttributeExists_ReturnsUserAttributeWithGivenPrimaryKey()
        {
            var userAttribute = fixture.Create<UserAttributeDb>();

            dynamoDbContext.Setup(
                x => x.LoadAsync<UserAttributeDb>(
                    userAttribute.UserUuid.ToLower(CultureInfo.CurrentCulture),
                    userAttribute.Name.ToLower(CultureInfo.CurrentCulture),
                    It.Is<DynamoDBOperationConfig>(c => c.OverrideTableName == TableName),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(userAttribute);

            var foundUserAttribute = await repository.FindByUserUuidAndNameAsync(userAttribute.UserUuid, userAttribute.Name);

            dynamoDbContext.VerifyAll();

            foundUserAttribute.Should().NotBeNull("because the user attribute exists")
                .And.BeEquivalentTo(userAttribute, options => options.ExcludingMissingMembers());
        }

        [Test]
        public void FindByUserUuidAndNameAsync_WhenUserUuidIsNull_ShouldThrowArgumentNullException()
        {
            repository.Awaiting(r => r.FindByUserUuidAndNameAsync(null, "name"))
                .Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("userUuid");
        }

        [Test]
        public void FindByUserUuidAndNameAsync_WhenNameIsNull_ShouldThrowArgumentNullException()
        {
            repository.Awaiting(r => r.FindByUserUuidAndNameAsync("userUuid", null))
                .Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("name");
        }

        [Test]
        public async Task FindByUserUuidAndNameAsync_WhenUserAttributeDoesNotExist_ShouldReturnNull()
        {
            dynamoDbContext.Setup(
                    x => x.LoadAsync<UserAttributeDb>(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.Is<DynamoDBOperationConfig>(c => c.OverrideTableName == TableName),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserAttributeDb)null);

            var foundUserAttribute = await repository.FindByUserUuidAndNameAsync("userUuid", "name");

            dynamoDbContext.VerifyAll();
            foundUserAttribute.Should().BeNull("because the user attribute does not exist");
        }

        [Test]
        public void FindByUserUuidAndNameAsync_WhenLoadAsyncThrowsAmazonDynamoDbException_ShouldThrowAmazonDynamoDbException()
        {
            dynamoDbContext.Setup(
                    x => x.LoadAsync<UserAttributeDb>(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.Is<DynamoDBOperationConfig>(c => c.OverrideTableName == TableName),
                        It.IsAny<CancellationToken>()))
                .ThrowsAsync(new AmazonDynamoDBException("exception message"));

            repository.Awaiting(r => r.FindByUserUuidAndNameAsync("userUuid", "name"))
                .Should().Throw<AmazonDynamoDBException>();
        }

        [Test]
        public void FindByUserUuidAsync_WhenUserUuidIsNull_ShouldThrowArgumentNullException()
        {
            repository.Awaiting(r => r.FindByUserUuidAsync(null))
                .Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("userUuid");
        }

        #endregion

        #region SearchAsync(UserAttributeFilter filter)

        [Test]
        public async Task SearchAsyncByFilter_WhenUserAttributesExist_ReturnsUserAttributes()
        {
            var userAttributes = fixture.CreateMany<UserAttributeDb>();
            SetUpPerformQueryMethod(userAttributes);

            var foundServiceAccount = await repository.SearchAsync(new UserAttributeFilter());

            mockQueryWrapper.VerifyAll();
            foundServiceAccount.Should().NotBeNull("because the user attributes exist")
                .And.BeEquivalentTo(userAttributes, options => options.ExcludingMissingMembers());
        }

        [Test]
        public async Task SearchAsyncByFilter_WhenFilterIsNull_SuccessSearchWithNullFilter()
        {
            var userAttributes = fixture.CreateMany<UserAttributeDb>();
            SetUpPerformQueryMethod(userAttributes);

            var foundUserAttributes = await repository.SearchAsync((UserAttributeFilter)null);

            mockQueryWrapper.VerifyAll();
            foundUserAttributes.Should().BeEquivalentTo(userAttributes);
        }

        [Test]
        public async Task SearchAsyncByFilter_WhenFilteringByUserUuid_SuccessSearchWithUserUuidFilter()
        {
            var userAttributes = fixture.CreateMany<UserAttributeDb>();
            var userAttributeFilter = new UserAttributeFilter { UserUuid = "userUuid" };
            SetUpPerformQueryMethod(userAttributes);

            var foundUserAttributes = await repository.SearchAsync(userAttributeFilter);

            mockQueryWrapper.VerifyAll();
            foundUserAttributes.Should().BeEquivalentTo(userAttributes);
        }

        [Test]
        public async Task SearchAsyncByFilter_WhenFilteringByUserUuidAndNames_SuccessSearchWithUserUuidAndNamesFilter()
        {
            var userAttributes = fixture.CreateMany<UserAttributeDb>();
            var userAttributeFilter = new UserAttributeFilter
            {
                UserUuid = "userUuid",
                Names = new List<string> { "name1", "name2" }
            };
            SetUpPerformQueryMethod(userAttributes);

            var foundUserAttributes = await repository.SearchAsync(userAttributeFilter);

            mockQueryWrapper.VerifyAll();
            foundUserAttributes.Should().BeEquivalentTo(userAttributes);
        }

        [Test]
        public async Task SearchAsyncByFilter_WhenFilteringByName_SuccessSearchWithNameFilter()
        {
            var userAttributes = fixture.CreateMany<UserAttributeDb>();
            var userAttributeFilter = new UserAttributeFilter { Name = "name" };
            SetUpPerformQueryMethod(userAttributes);

            var foundUserAttributes = await repository.SearchAsync(userAttributeFilter);

            mockQueryWrapper.VerifyAll();
            foundUserAttributes.Should().BeEquivalentTo(userAttributes);
        }

        #endregion

        #region SearchAsync(Cursor<UserAttributeFilter> cursor)

        [Test]
        public void SearchAsync_WhenCursorIsNull_ShouldThrowArgumentNullException()
        {
            repository.Awaiting(c => c.SearchAsync((Cursor<UserAttributeFilter>)null))
                .Should().Throw<ArgumentNullException>();
        }

        [Test]
        public async Task SearchAsync_WhenFilterIsNull_SuccessSearchWithNullFilter()
        {
            var userAttributes = fixture.CreateMany<UserAttributeDb>();
            var expectedfirstItemToken = fixture.Create<string>();
            var expectedlastItemToken = fixture.Create<string>();
            var userAttributeFilter = (UserAttributeFilter)null;
            var cursor = new Cursor<UserAttributeFilter>(0, userAttributeFilter);
            SetUpPerformQueryMethod(userAttributes, expectedfirstItemToken, expectedlastItemToken);

            var (foundUserAttributes, firstItemToken, lastItemToken) = await repository.SearchAsync(cursor);

            mockQueryWrapper.VerifyAll();
            foundUserAttributes.Should().BeEquivalentTo(userAttributes);

            using (new AssertionScope())
            {
                foundUserAttributes.Should().BeEquivalentTo(userAttributes);
                firstItemToken.Should().BeEquivalentTo(expectedfirstItemToken);
                lastItemToken.Should().BeEquivalentTo(expectedlastItemToken);
            }
        }

        [Test]
        public async Task SearchAsync_WhenFilteringByUserUuid_SuccessSearchWithUserUuidFilter()
        {
            var userAttributes = fixture.CreateMany<UserAttributeDb>();
            var expectedfirstItemToken = fixture.Create<string>();
            var expectedlastItemToken = fixture.Create<string>();
            var userAttributeFilter = new UserAttributeFilter { UserUuid = "userUuid" };
            var cursor = new Cursor<UserAttributeFilter>(0, userAttributeFilter);
            SetUpPerformQueryMethod(userAttributes, expectedfirstItemToken, expectedlastItemToken);

            var (foundUserAttributes, firstItemToken, lastItemToken) = await repository.SearchAsync(cursor);

            mockQueryWrapper.VerifyAll();
            foundUserAttributes.Should().BeEquivalentTo(userAttributes);

            using (new AssertionScope())
            {
                foundUserAttributes.Should().BeEquivalentTo(userAttributes);
                firstItemToken.Should().BeEquivalentTo(expectedfirstItemToken);
                lastItemToken.Should().BeEquivalentTo(expectedlastItemToken);
            }
        }

        [Test]
        public async Task SearchAsync_WhenFilteringByName_SuccessSearchWithNameFilter()
        {
            var userAttributes = fixture.CreateMany<UserAttributeDb>();
            var expectedfirstItemToken = fixture.Create<string>();
            var expectedlastItemToken = fixture.Create<string>();
            var userAttributeFilter = new UserAttributeFilter { Name = "name" };
            var cursor = new Cursor<UserAttributeFilter>(0, userAttributeFilter);
            SetUpPerformQueryMethod(userAttributes, expectedfirstItemToken, expectedlastItemToken);

            var (foundUserAttributes, firstItemToken, lastItemToken) = await repository.SearchAsync(cursor);

            mockQueryWrapper.VerifyAll();
            foundUserAttributes.Should().BeEquivalentTo(userAttributes);

            using (new AssertionScope())
            {
                foundUserAttributes.Should().BeEquivalentTo(userAttributes);
                firstItemToken.Should().BeEquivalentTo(expectedfirstItemToken);
                lastItemToken.Should().BeEquivalentTo(expectedlastItemToken);
            }
        }

        [Test]
        public async Task SearchAsync_WhenFilteringByUserUuidAndNames_SuccessSearchWithUserUuidAndNamesFilter()
        {
            var userAttributes = fixture.CreateMany<UserAttributeDb>();
            var expectedfirstItemToken = fixture.Create<string>();
            var expectedlastItemToken = fixture.Create<string>();
            var userAttributeFilter = new UserAttributeFilter
            {
                UserUuid = "userUuid",
                Names = new List<string> { "name1", "name2" }
            };
            var cursor = new Cursor<UserAttributeFilter>(0, userAttributeFilter);
            SetUpPerformQueryMethod(userAttributes, expectedfirstItemToken, expectedlastItemToken);

            var (foundUserAttributes, firstItemToken, lastItemToken) = await repository.SearchAsync(cursor);

            mockQueryWrapper.VerifyAll();
            foundUserAttributes.Should().BeEquivalentTo(userAttributes);

            using (new AssertionScope())
            {
                foundUserAttributes.Should().BeEquivalentTo(userAttributes);
                firstItemToken.Should().BeEquivalentTo(expectedfirstItemToken);
                lastItemToken.Should().BeEquivalentTo(expectedlastItemToken);
            }
        }

        #endregion

        #region Save

        [Test]
        public void SaveAsync_WhenItemIsNull_ShouldThrowArgumentNullException()
        {
            repository.Awaiting(r => r.SaveAsync(null))
                .Should().Throw<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("item");
        }

        [Test]
        public async Task SaveAsyncSuccess()
        {
            UserAttributeDb savedUserAttribute = null;

            dynamoDbContext.Setup(
                    x => x.SaveAsync(
                        It.IsAny<UserAttributeDb>(),
                        It.Is<DynamoDBOperationConfig>(c => c.OverrideTableName == TableName),
                        It.IsAny<CancellationToken>()))
                .Callback<UserAttributeDb, DynamoDBOperationConfig, CancellationToken>((userAttribute, config, cancellation) => savedUserAttribute = userAttribute);

            var userAttributeDb = fixture.Create<UserAttributeDb>();

            await repository.SaveAsync(userAttributeDb);

            dynamoDbContext.VerifyAll();

            savedUserAttribute.Should().BeEquivalentTo(userAttributeDb);
        }

        [Test]
        public void SaveThrowsUpdateConflictExceptionIfConditionalCheckFails()
        {
            dynamoDbContext.Setup(
                db => db.SaveAsync(
                    It.IsAny<UserAttributeDb>(),
                    It.Is<DynamoDBOperationConfig>(c => c.OverrideTableName == TableName),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ConditionalCheckFailedException("Error message"));

            var userAttribute = fixture.Create<UserAttributeDb>();

            repository.Awaiting(r => r.SaveAsync(userAttribute))
                .Should().Throw<UpdateConflictException>()
                .WithInnerException<ConditionalCheckFailedException>();
        }

        [Test]
        public void SaveThrowsUpdateConflictExceptionWithoutInnerExceptionIfConditionalCheckFails()
        {
            dynamoDbContext.Setup(
                db => db.SaveAsync(
                    It.IsAny<UserAttributeDb>(),
                    It.Is<DynamoDBOperationConfig>(c => c.OverrideTableName == TableName),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ConditionalCheckFailedException("Error message"));

            var userAttribute = fixture.Create<UserAttributeDb>();

            repository.Awaiting(r => r.SaveAsync(userAttribute))
                .Should().Throw<UpdateConflictException>();
        }

        [Test]
        public async Task SaveNeedsConsistentReadToFind()
        {
            UserAttributeDb savedUserAttribute = fixture.Build<UserAttributeDb>()
                .With(a => a.Version, 1)
                .Create();

            dynamoDbContext.Setup(
                    x => x.LoadAsync<UserAttributeDb>(
                        savedUserAttribute.UserUuid,
                        savedUserAttribute.Name,
                        It.Is<DynamoDBOperationConfig>(c => c.OverrideTableName == TableName),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new UserAttributeDb { Version = 1 });

            dynamoDbContext.Setup(
                    x => x.LoadAsync<UserAttributeDb>(
                        savedUserAttribute.UserUuid,
                        savedUserAttribute.Name,
                        It.Is<DynamoDBOperationConfig>(c => c.OverrideTableName == TableName),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(savedUserAttribute);

            await repository.SaveAsync(savedUserAttribute);

            dynamoDbContext.VerifyAll();
            savedUserAttribute.Should().BeEquivalentTo(savedUserAttribute);
        }

        [Test]
        public async Task SaveReturnsTheUpdatedUserAttribute()
        {
            var updatedOn = DateTimeOffset.UtcNow;
            var originalUserAttributeDb = fixture.Build<UserAttributeDb>()
                .With(userAttribute => userAttribute.Version, 1)
                .With(userAttribute => userAttribute.UpdatedOn, updatedOn)
                .Create();

            var savedDynamoUserAttribute = fixture.Build<UserAttributeDb>()
                .With(appAccount => appAccount.UserUuid, originalUserAttributeDb.UserUuid)
                .With(appAccount => appAccount.Name, originalUserAttributeDb.Name)
                .With(appAccount => appAccount.Version, 2)
                .With(appAccount => appAccount.UpdatedOn, updatedOn)
                .Create();

            dynamoDbContext.Setup(
                    x => x.LoadAsync<UserAttributeDb>(
                        savedDynamoUserAttribute.UserUuid,
                        savedDynamoUserAttribute.Name,
                        It.Is<DynamoDBOperationConfig>(c => c.OverrideTableName == TableName),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(savedDynamoUserAttribute);

            var returnedUserAttribute = await repository.SaveAsync(originalUserAttributeDb);

            dynamoDbContext.VerifyAll();

            returnedUserAttribute.Should().BeEquivalentTo(savedDynamoUserAttribute, options => options.ExcludingMissingMembers());
        }

        #endregion

        #region Delete

        [Test]
        public async Task DeleteCallsDeleteOnDynamoDb()
        {
            var userAttributeToDelete = GetExistedUserAttributeDbViaLoad();

            dynamoDbContext.Setup(
                x => x.DeleteAsync(
                    userAttributeToDelete,
                    It.Is<DynamoDBOperationConfig>(c => c.OverrideTableName == TableName),
                    It.IsAny<CancellationToken>()));

            await repository.DeleteAsync(userAttributeToDelete.UserUuid, userAttributeToDelete.Name);

            dynamoDbContext.VerifyAll();
        }

        [Test]
        public async Task DeleteDoesNothingIfUserAttributeDoesNotExist()
        {
            const string userUuid = "userUuid";
            const string name = "name";

            dynamoDbContext.Setup(
                    x => x.LoadAsync<UserAttributeDb>(
                        userUuid.ToLower(CultureInfo.CurrentCulture),
                        name.ToLower(CultureInfo.CurrentCulture),
                        It.Is<DynamoDBOperationConfig>(c => c.OverrideTableName == TableName),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserAttributeDb)null);

            await repository.DeleteAsync("userUuid", "userUuid");

            dynamoDbContext.Verify(
                db => db.DeleteAsync(
                    It.IsAny<UserAttributeDb>(),
                    It.IsAny<DynamoDBOperationConfig>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Test]
        public void DeleteAsync_WhenUserUuidIsNull_ShouldThrowArgumentNullException()
        {
            repository.Awaiting(r => r.DeleteAsync(null, "name"))
                .Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("userUuid");
        }

        [Test]
        public void DeleteAsync_WhenNameIsNull_ShouldThrowArgumentNullException()
        {
            repository.Awaiting(r => r.DeleteAsync("userUuid", null))
                .Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("name");
        }

        #endregion

        #region Private methods

        private void SetUpPerformQueryMethod(IEnumerable<UserAttributeDb> userAttributes, string firstItemToken = null, string lastItemToken = null)
        {
            firstItemToken ??= fixture.Create<string>();
            lastItemToken ??= fixture.Create<string>();

            mockQueryWrapper.Setup(q => q.PerformQueryAsync(
                It.IsAny<string>(),
                It.IsAny<Cursor<UserAttributeFilter>>(),
                It.IsAny<QueryOperationConfig>()))
                .ReturnsAsync((userAttributes, firstItemToken, lastItemToken));
        }

        private UserAttributeDb GetExistedUserAttributeDbViaLoad()
        {
            var userAttribute = fixture.Create<UserAttributeDb>();

            SetUpLoadAsyncToReturnUserAttribute(userAttribute);

            return userAttribute;
        }

        private void SetUpLoadAsyncToReturnUserAttribute(UserAttributeDb userAttribute)
        {
            dynamoDbContext.Setup(
                    x => x.LoadAsync<UserAttributeDb>(
                        userAttribute.UserUuid.ToLower(CultureInfo.CurrentCulture),
                        userAttribute.Name.ToLower(CultureInfo.CurrentCulture),
                        It.Is<DynamoDBOperationConfig>(c => c.OverrideTableName == TableName),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(userAttribute);
        }

        #endregion
    }
}
