using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Refinitiv.Aaa.GuissApi.Data.Interfaces;
using Refinitiv.Aaa.GuissApi.Data.Models;
using Refinitiv.Aaa.GuissApi.Facade.Helpers;
using Refinitiv.Aaa.GuissApi.Facade.Mapping;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.UserAttribute;
using Refinitiv.Aaa.Interfaces.Headers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Newtonsoft.Json.Linq;
using Refinitiv.Aaa.GuissApi.Facade.Interfaces;

namespace Refinitiv.Aaa.GuissApi.Facade.Tests.Helpers
{
    [TestFixture]
    public class UserAttributeHelperTests
    {
        private readonly IFixture fixture = new Fixture();
        private UserAttributeHelper userAttributeHelper;
        private Mock<IUserAttributeRepository> userAttributeRepository;
        private Mock<IAaaRequestHeaders> aaaRequestHeaders;
        private Mock<IUserAttributeProvider> userAttributeProvider;
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
            aaaRequestHeaders = new Mock<IAaaRequestHeaders>();
            userAttributeProvider = new Mock<IUserAttributeProvider>();

            userAttributeHelper = new UserAttributeHelper(
                userAttributeRepository.Object,
                mapper,
                aaaRequestHeaders.Object,
                userAttributeProvider.Object
                );
        }

        [Test]
        public async Task GetAllByUserUuidAsync_ShouldCallGetUserAttributesAsyncAndReturnJObject()
        {
            var attributes = fixture.CreateMany<UserAttributeDetails>();
            var userUuid = fixture.Create<string>();

            userAttributeProvider.Setup(x =>
                    x.GetUserAttributesAsync(userUuid))
                .ReturnsAsync(attributes);

            var result = await userAttributeHelper.GetAllByUserUuidAsync(userUuid);

            userAttributeProvider.VerifyAll();

            result.Should().BeOfType<JObject>("because a result is always returned");
        }

        [Test]
        public async Task GetAttributesByUserUuidAsync_ShouldCallGetUserAttributesAsyncAndReturnJObject()
        {
            var attributes = fixture.CreateMany<UserAttributeDetails>();
            var userUuid = fixture.Create<string>();
            var attributesNames = "one,two,three";
            var attributesList = new List<string> {"one", "two", "three"};

            userAttributeProvider.Setup(x =>
                    x.GetUserAttributesAsync(userUuid, attributesList))
                .ReturnsAsync(attributes);

            var result = await userAttributeHelper.GetAttributesByUserUuidAsync(userUuid, attributesNames);

            userAttributeRepository.VerifyAll();

            result.Should().BeOfType<JObject>("because a result is always returned");
        }

        [Test]
        public async Task GetAttributesByUserUuidAsync_OnNullAttributes_ShouldThrowArgumentNullException()
        {
            var userUuid = fixture.Create<string>();
            Func<Task> act = async () => await userAttributeHelper.GetAttributesByUserUuidAsync(userUuid, null);
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

            [Test]
        public void InsertAsyncThrowsExceptionIfArgumentIsNull()
        {
            UserAttributeDetails userAttributeDetails = null;

            Assert.ThrowsAsync<ArgumentNullException>(() => userAttributeHelper.InsertAsync(userAttributeDetails));
        }

        [Test]
        public void UpdateAsyncThrowsExceptionIfArgumentIsNull()
        {
            UserAttribute userAttribute = null;

            string value = "someValue";

            Assert.ThrowsAsync<ArgumentNullException>(() => userAttributeHelper.UpdateAsync(userAttribute, value));
        }

        [Test]
        public async Task InsertAsyncCallSaveChangesIfArgumentIsNotNull()
        {
            var userAttributeDetails = new UserAttributeDetails();

            userAttributeRepository.Setup(x => x.SaveAsync(It.IsAny<UserAttributeDb>())).ReturnsAsync(new UserAttributeDb());

            await userAttributeHelper.InsertAsync(userAttributeDetails);

            userAttributeRepository.Verify(x => x.SaveAsync(It.IsAny<UserAttributeDb>()), Times.Once());
        }

        [Test]
        public async Task UpdateAsyncCallSaveChangesIfArgumentIsNotNull()
        {
            UserAttribute userAttribute = new UserAttribute();

            string value = "someValue";

            userAttributeRepository.Setup(x => x.SaveAsync(It.IsAny<UserAttributeDb>())).ReturnsAsync(new UserAttributeDb());

            await userAttributeHelper.UpdateAsync(userAttribute, value);

            userAttributeRepository.Verify(x => x.SaveAsync(It.IsAny<UserAttributeDb>()), Times.Once());
        }

        [Test]
        public async Task InsertAsyncReturnsSavedAttributeIfArgumentIsNotNull()
        {
            var userAttributeDetails = new UserAttributeDetails();

            var userAttributeDb = new UserAttributeDb() { Name = "CustomName", UserUuid = "CustomUuid", Value = "CustomValue" };

            userAttributeRepository.Setup(x => x.SaveAsync(It.IsAny<UserAttributeDb>())).ReturnsAsync(userAttributeDb);

            var result = await userAttributeHelper.InsertAsync(userAttributeDetails);

            result.Should().BeEquivalentTo(mapper.Map<UserAttributeDb, UserAttribute>(userAttributeDb));
        }

        [Test]
        public async Task UpdateAsyncReturnsUpdatedAttributeIfArgumentIsNotNull()
        {
            string value = "someValue";

            UserAttribute userAttribute = new UserAttribute() { Name = "CustomName", UserUuid = "CustomUuid", Value = value };

            var userAttributeDb = mapper.Map<UserAttribute, UserAttributeDb>(userAttribute);

            userAttributeRepository.Setup(x => x.SaveAsync(It.IsAny<UserAttributeDb>())).ReturnsAsync(userAttributeDb);

            var result = await userAttributeHelper.UpdateAsync(userAttribute, value);

            result.Should().BeEquivalentTo(userAttribute);
        }

        [Test]
        public async Task DeleteUserAttributeAsync_WhenDeletedSuccessfully()
        {
            var userUuid = fixture.Create<string>();
            var name = fixture.Create<string>();

            userAttributeRepository.Setup(x => x.DeleteAsync(userUuid, name));

            await userAttributeHelper.DeleteUserAttributeAsync(userUuid, name);

            userAttributeRepository.VerifyAll();
        }

        [Test]
        public async Task GetAttributesByUserNamespacesAndUuidAsync_OnNullNamespaces_ShouldThrowArgumentNullException()
        {
            var userUuid = fixture.Create<string>();
            Func<Task> act = async () => await userAttributeHelper.GetAttributesByUserNamespacesAndUuidAsync(userUuid, null);
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Test]
        public async Task GetAttributesByUserNamespacesAndUuidAsync_ShouldCallSearchAsyncWithUserUuidFilterAndReturnJObject()
        {
            var attributes = fixture.CreateMany<UserAttributeDb>();
            var userUuid = fixture.Create<string>();
            var attributesNamespaces = "one,two,three";

            userAttributeRepository.Setup(x =>
                    x.SearchAsync(It.IsAny<UserAttributeFilter>()))
                .ReturnsAsync(attributes);

            var result = await userAttributeHelper.GetAttributesByUserNamespacesAndUuidAsync(userUuid, attributesNamespaces);

            userAttributeRepository.VerifyAll();

            result.Should().BeOfType<JObject>("because a result is always returned");
        }
    }
}
