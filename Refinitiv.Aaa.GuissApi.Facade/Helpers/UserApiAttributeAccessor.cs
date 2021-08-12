using System.Net.Http;
using Refinitiv.Aaa.Foundation.ApiClient.Constants;
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
        public UserApiAttributeAccessor(
            IUserAttributeConfigHelper userAttributeConfigHelper,
            IHttpClientFactory httpClientFactory,
            IAaaRequestHeaders aaaRequestHeaders)
            : base(httpClientFactory, aaaRequestHeaders,
                userAttributeConfigHelper.GetUserAttributeApiConfig(ServiceNames.UserApi))
        {
        }
    }
}
