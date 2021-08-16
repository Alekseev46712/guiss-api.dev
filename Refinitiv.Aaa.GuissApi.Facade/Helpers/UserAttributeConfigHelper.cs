using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Refinitiv.Aaa.GuissApi.Facade.Interfaces;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.Configuration;

namespace Refinitiv.Aaa.GuissApi.Facade.Helpers
{
    /// <inheritdoc />
    public class UserAttributeConfigHelper : IUserAttributeConfigHelper
    {
        private IEnumerable<UserAttributeApiConfig> configs;
        private readonly string configFilePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAttributeConfigHelper"/> class.
        /// </summary>
        public UserAttributeConfigHelper()
        {
            configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "attributes.json");
            LoadConfigFile();
        }

        /// <inheritdoc />
        public UserAttributeApiConfig GetUserAttributeApiConfig(string apiName)
        {
            return configs.FirstOrDefault(c => c.ApiName == apiName);
        }

        private void LoadConfigFile()
        {
            using (var r = new StreamReader(configFilePath))
            {
                var json = r.ReadToEnd();
                configs = JsonConvert.DeserializeObject<IEnumerable<UserAttributeApiConfig>>(json);
            }
        }
    }
}
