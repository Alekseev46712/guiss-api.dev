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
using System.Text;
using System.Threading.Tasks;

namespace Refinitiv.Aaa.GuissApi.Facade.Tests.Helpers
{
    [TestFixture]
    public class UserAttributeHelperTests
    {
        private UserAttributeHelper userAttributeHelper;
        private Mock<IUserAttributeRepository> userAttributeRepository;
        private Mock<IAaaRequestHeaders> aaaRequestHeaders;
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

            userAttributeHelper = new UserAttributeHelper(
                userAttributeRepository.Object,
                mapper,
                aaaRequestHeaders.Object
                );
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

    }
}
