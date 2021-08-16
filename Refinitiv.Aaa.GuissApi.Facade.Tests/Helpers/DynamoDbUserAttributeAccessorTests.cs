using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Refinitiv.Aaa.GuissApi.Data.Interfaces;
using Refinitiv.Aaa.GuissApi.Data.Models;
using Refinitiv.Aaa.GuissApi.Facade.Helpers;
using Refinitiv.Aaa.GuissApi.Facade.Mapping;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.UserAttribute;

namespace Refinitiv.Aaa.GuissApi.Facade.Tests.Helpers
{
    [TestFixture]
    public class DynamoDbUserAttributeAccessorTests
    {
        private readonly IFixture fixture = new Fixture();
        private DynamoDbUserAttributeAccessor dynamoDbAccessor;
        private Mock<IUserAttributeRepository> userAttributeRepository;
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

            dynamoDbAccessor = new DynamoDbUserAttributeAccessor(
                userAttributeRepository.Object,
                mapper
            );
        }

        [Test]
        public async Task GetAllByUserUuidAsync_ShouldCallGetUserAttributesAsyncAndReturnJObject()
        {
            var attributes = fixture.CreateMany<UserAttributeDb>();
            var userUuid = fixture.Create<string>();
            var attributeNames = fixture.CreateMany<string>();

            userAttributeRepository.Setup(x =>
                    x.SearchAsync(It.Is<UserAttributeFilter>(a =>
                        a.UserUuid == userUuid && Equals(a.Names, attributeNames))))
                .ReturnsAsync(attributes);

            var result = await dynamoDbAccessor.GetUserAttributesAsync(userUuid, attributeNames);

            userAttributeRepository.VerifyAll();

            result.Should().BeOfType<List<UserAttributeDetails>>("because a result is always returned");
        }
    }
}
