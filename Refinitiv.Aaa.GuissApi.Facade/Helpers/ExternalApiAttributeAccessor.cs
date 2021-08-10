using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Refinitiv.Aaa.Foundation.ApiClient.Exceptions;
using Refinitiv.Aaa.Foundation.ApiClient.Extensions;
using Refinitiv.Aaa.GuissApi.Facade.Exceptions;
using Refinitiv.Aaa.GuissApi.Facade.Interfaces;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.Configuration;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.UserAttribute;
using Refinitiv.Aaa.Interfaces.Headers;

namespace Refinitiv.Aaa.GuissApi.Facade.Helpers
{
    /// <inheritdoc />
    public abstract class ExternalApiAttributeAccessor : IExternalUserAttributeAccessor
    {
        private readonly UserAttributeApiConfig config;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IAaaRequestHeaders aaaRequestHeaders;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalApiAttributeAccessor"/> class.
        /// </summary>
        /// <param name="userAttributeConfigHelper">The user attribute config helper.</param>
        /// <param name="httpClientFactory">The HttpClientFactory.</param>
        /// <param name="aaaRequestHeaders">The AAA request headers.</param>
        protected ExternalApiAttributeAccessor(
            IUserAttributeConfigHelper userAttributeConfigHelper,
            IHttpClientFactory httpClientFactory,
            IAaaRequestHeaders aaaRequestHeaders)
        {
            config = userAttributeConfigHelper.GetUserAttributeApiConfig(ApiName);
            this.httpClientFactory = httpClientFactory;
            this.aaaRequestHeaders = aaaRequestHeaders;
        }

        /// <summary>
        /// Name of the external API.
        /// </summary>
        protected abstract string ApiName { get; }

        /// <inheritdoc />
        public IEnumerable<string> DefaultAttributes => config.Attributes.Keys;

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

            var response = await GetApiResponseAsync(userUuid);

            foreach (var attributeName in attributeNames)
            {
                var attributeConfig = config.Attributes[attributeName];
                var attributeValues = response.SelectTokens(attributeConfig.ResponsePath);

                if (!attributeValues.Any())
                {
                    throw new InvalidResponsePathException(attributeName, attributeConfig.ResponsePath, config.ApiName);
                }

                result.Add(new UserAttributeDetails()
                {
                    UserUuid = userUuid,
                    Name = attributeName,
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
