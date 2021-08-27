using Newtonsoft.Json.Linq;
using Refinitiv.Aaa.Foundation.ApiClient.Exceptions;
using Refinitiv.Aaa.Foundation.ApiClient.Extensions;
using Refinitiv.Aaa.Foundation.ApiClient.Interfaces;
using Refinitiv.Aaa.GuissApi.Facade.Exceptions;
using Refinitiv.Aaa.GuissApi.Facade.Interfaces;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.Configuration;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.UserAttribute;
using Refinitiv.Aaa.Interfaces.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Refinitiv.Aaa.GuissApi.Facade.Helpers
{
    /// <inheritdoc />
    public abstract class ExternalApiAttributeAccessor : IExternalUserAttributeAccessor
    {
        private UserAttributeApiConfig config;

        private readonly string apiName;
        private readonly IUserAttributeConfigHelper userAttributeConfigHelper;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IAaaRequestHeaders aaaRequestHeaders;
        private readonly IDataCacheService cacheService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalApiAttributeAccessor"/> class.
        /// </summary>
        /// <param name="httpClientFactory">The HttpClientFactory.</param>
        /// <param name="aaaRequestHeaders">The AAA request headers.</param>
        /// <param name="userAttributeConfigHelper">The user attribute config helper.</param>
        /// <param name="apiName">The api name.</param>
        /// <param name="cacheService">The caching service.</param>
        protected ExternalApiAttributeAccessor(
            IHttpClientFactory httpClientFactory,
            IAaaRequestHeaders aaaRequestHeaders,
            IUserAttributeConfigHelper userAttributeConfigHelper,
            string apiName,
            IDataCacheService cacheService)
        {
            this.apiName = apiName;
            this.userAttributeConfigHelper = userAttributeConfigHelper;
            this.httpClientFactory = httpClientFactory;
            this.aaaRequestHeaders = aaaRequestHeaders;
            this.cacheService = cacheService;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<string>> GetDefaultAttributesAsync()
        {
            config = await userAttributeConfigHelper.GetUserAttributeApiConfigAsync(apiName);

            return config.Attributes.Select(a => a.Name);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<UserAttributeDetails>> GetUserAttributesAsync(
            string userUuid,
            IEnumerable<string> attributeNames)
        {
            var result = new List<UserAttributeDetails>();

            if (attributeNames == null)
            {
                return result;
            }

            config = await userAttributeConfigHelper.GetUserAttributeApiConfigAsync(apiName);

            var response = await cacheService.GetValue(config.ApiName, userUuid, GetApiResponseAsync);

            foreach (var attributeName in attributeNames)
            {
                var attributeConfig = config.Attributes.First(a =>
                    string.Equals(a.Name, attributeName, StringComparison.CurrentCultureIgnoreCase));
                var attributeValues = response.SelectTokens(attributeConfig.ResponsePath).ToList();

                if (!attributeValues.Any())
                {
                    throw new InvalidResponsePathException(attributeName, attributeConfig.ResponsePath, config.ApiName);
                }

                result.Add(new UserAttributeDetails()
                {
                    UserUuid = userUuid,
                    Name = attributeConfig.Name,
                    Value = string.Join(" ", attributeValues)
                });
            }

            return result;
        }

        private async Task<JObject> GetApiResponseAsync(string userUuid)
        {
            var client = httpClientFactory.CreateClient(config.ApiName);
            client.AddAaaHeaders(aaaRequestHeaders);

            var uriString = string.Format(config.UrlTemplate, userUuid);
            var uri = new Uri(uriString);

            using (var response = await client.GetAsync(uri))
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new InvalidHttpResponseException(config.ApiName, response.StatusCode, responseContent);
                }

                return JObject.Parse(responseContent);
            }
        }
    }
}
