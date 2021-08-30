using Newtonsoft.Json;
using Refinitiv.Aaa.Ciam.SharedLibrary.Services.Interfaces;
using Refinitiv.Aaa.GuissApi.Facade.Interfaces;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Refinitiv.Aaa.GuissApi.Facade.Helpers
{
    /// <inheritdoc />
    public class UserAttributeConfigHelper : IUserAttributeConfigHelper
    {
        private IEnumerable<UserAttributeApiConfig> configs;
        private readonly ParameterStoreConfig parameterStoreConfig;
        private readonly AwsConfig awsConfig;
        private readonly IParameterStoreService parameterStoreService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAttributeConfigHelper"/> class.
        /// </summary>
        /// <param name="parameterStoreConfig">The parameter store configuration.</param>
        /// <param name="awsConfig">The aws configuration.</param>
        /// <param name="parameterStoreService">Parameter store service.</param>
        public UserAttributeConfigHelper(
            IOptions<ParameterStoreConfig> parameterStoreConfig,
            IOptions<AwsConfig> awsConfig,
            IParameterStoreService parameterStoreService)
        {
            this.parameterStoreConfig = parameterStoreConfig.Value;
            this.awsConfig = awsConfig.Value;
            this.parameterStoreService = parameterStoreService;
        }

        /// <inheritdoc />
        public async Task<UserAttributeApiConfig> GetUserAttributeApiConfigAsync(string apiName)
        {
            await GetParameterStoreValueAsync();
            return configs.FirstOrDefault(c => c.ApiName == apiName);
        }

        private async Task GetParameterStoreValueAsync()
        {
            var parameterStoreName = string.Join("/", awsConfig.ParameterStorePath,
                parameterStoreConfig.UserAttributeApiConfigParameterStoreName);

            var jsonParameter = await parameterStoreService.GetParameterAsync(parameterStoreName);

            configs = JsonConvert.DeserializeObject<IEnumerable<UserAttributeApiConfig>>(jsonParameter);
        }
    }
}
