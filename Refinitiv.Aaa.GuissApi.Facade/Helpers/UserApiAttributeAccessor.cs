using System.Net.Http;
using Refinitiv.Aaa.Foundation.ApiClient.Constants;
using Refinitiv.Aaa.Foundation.ApiClient.Interfaces;
using Refinitiv.Aaa.GuissApi.Facade.Interfaces;
using Refinitiv.Aaa.Interfaces.Headers;

namespace Refinitiv.Aaa.GuissApi.Facade.Helpers
{
    /// <inheritdoc />
    public class UserApiAttributeAccessor : ExternalApiAttributeAccessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserApiAttributeAccessor"/> class.
        /// </summary>
        /// <param name="userAttributeConfigHelper">The user attribute config helper.</param>
        /// <param name="httpClientFactory">The HttpClientFactory.</param>
        /// <param name="aaaRequestHeaders">The AAA request headers.</param>
        /// <param name="cacheService">The caching service.</param>
        public UserApiAttributeAccessor(
            IUserAttributeConfigHelper userAttributeConfigHelper,
            IHttpClientFactory httpClientFactory,
            IAaaRequestHeaders aaaRequestHeaders,
            IDataCacheService cacheService)
            : base(httpClientFactory,
                aaaRequestHeaders,
                userAttributeConfigHelper.GetUserAttributeApiConfig(ServiceNames.UserApi).Result,
                cacheService)
        {
        }
    }
}
