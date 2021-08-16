using System.Net.Http;
using Moq;
using NUnit.Framework;
using Refinitiv.Aaa.Foundation.ApiClient.Constants;
using Refinitiv.Aaa.Foundation.ApiClient.Interfaces;
using Refinitiv.Aaa.GuissApi.Facade.Helpers;
using Refinitiv.Aaa.GuissApi.Facade.Interfaces;
using Refinitiv.Aaa.Interfaces.Headers;

namespace Refinitiv.Aaa.GuissApi.Facade.Tests.Helpers
{
    [TestFixture]
    public class UserApiAttributeAccessorTests
    {
        private Mock<IUserAttributeConfigHelper> userAttributeConfigHelper;
        private Mock<IHttpClientFactory> httpClientFactory;
        private Mock<IAaaRequestHeaders> aaaRequestHeaders;
        private Mock<IDataCacheService> cacheService;

        [SetUp]
        public void Setup()
        {
            userAttributeConfigHelper = new Mock<IUserAttributeConfigHelper>();
            httpClientFactory = new Mock<IHttpClientFactory>();
            aaaRequestHeaders = new Mock<IAaaRequestHeaders>();
            cacheService = new Mock<IDataCacheService>();
        }

        [Test]
        public void UserApiAttributeAccessorConstructor_ShouldCallGetUserAttributeApiConfigWithUserApiServiceName()
        {
            userAttributeConfigHelper.Setup(x => x.GetUserAttributeApiConfig(ServiceNames.UserApi));

            new UserApiAttributeAccessor(
                userAttributeConfigHelper.Object, httpClientFactory.Object, aaaRequestHeaders.Object,
                cacheService.Object);

            userAttributeConfigHelper.VerifyAll();
        }
    }
}
