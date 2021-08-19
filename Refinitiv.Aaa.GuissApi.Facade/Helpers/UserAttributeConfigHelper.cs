using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Refinitiv.Aaa.Ciam.SharedLibrary.Services.Interfaces;
using Refinitiv.Aaa.GuissApi.Facade.Interfaces;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Refinitiv.Aaa.GuissApi.Facade.Helpers
{
    /// <inheritdoc />
    public class UserAttributeConfigHelper : IUserAttributeConfigHelper
    {
        private IEnumerable<UserAttributeApiConfig> configs;
        private readonly IConfiguration configuration;
        private readonly IParameterStoreService parameterStoreService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAttributeConfigHelper"/> class.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        /// <param name="parameterStoreService">Parameter store service.</param>
        public UserAttributeConfigHelper(IConfiguration configuration, IParameterStoreService parameterStoreService)
        {
            this.configuration = configuration;
            this.parameterStoreService = parameterStoreService;
        }

        /// <inheritdoc />
        public async Task<UserAttributeApiConfig> GetUserAttributeApiConfig(string apiName)
        {
            await GetParameterStoreValue();
            return configs.FirstOrDefault(c => c.ApiName == apiName);
        }

        private async Task GetParameterStoreValue()
        {
            var jsonParameter = await parameterStoreService.GetParameterAsync(configuration["ParameterStore:UserAttributeApiConfigParameterStorePath"]);
            configs = JsonConvert.DeserializeObject<IEnumerable<UserAttributeApiConfig>>(jsonParameter);
        }
    }
}
