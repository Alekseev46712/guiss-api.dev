using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Refinitiv.Aaa.GuissApi.Facade.Interfaces;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.Configuration;

namespace Refinitiv.Aaa.GuissApi.Facade.Helpers
{
    /// <inheritdoc />
    public class CachingManager: ICachingManager
    {
        private JObject userData;
        private readonly IMemoryCache cache;
        private readonly CacheConfig cacheConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachingManager"/> class.
        /// </summary>
        /// <param name="cache">Memory cache.</param>
        /// <param name="appSettings">Application configuration.</param>
        public CachingManager(IMemoryCache cache, IOptions<AppSettingsConfig> appSettings)
        {
            this.cache = cache;
            cacheConfig = appSettings.Value.Cache;
        }

        /// <inheritdoc />
        public JObject GetUserData(string apiName)
        {
            cache.TryGetValue(apiName, out userData);
            return userData;
        }

        /// <inheritdoc />
        public void SetUserData(string apiName, JObject value)
        {
            var options = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(cacheConfig.ExpirationMinutes));
            cache.Set(apiName, value, options);
        }
    }
}
