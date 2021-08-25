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
        private readonly IParameterStoreService parameterStoreService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAttributeConfigHelper"/> class.
        /// </summary>
        /// <param name="parameterStoreConfig">The parameter store configuration.</param>
        /// <param name="parameterStoreService">Parameter store service.</param>
        public UserAttributeConfigHelper(
            IOptions<ParameterStoreConfig> parameterStoreConfig,
            IParameterStoreService parameterStoreService)
        {
            this.parameterStoreConfig = parameterStoreConfig.Value;
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
            var jsonParameter = await parameterStoreService
                .GetParameterAsync(parameterStoreConfig.UserAttributeApiConfigParameterStorePath);
            configs = JsonConvert.DeserializeObject<IEnumerable<UserAttributeApiConfig>>(jsonParameter);
        }
    }
}
