using Enyim.Caching;
using Enyim.Caching.Memcached;
using Enyim.Caching.Memcached.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Refinitiv.Aaa.GuissApi.Facade.Interfaces;
using Refinitiv.Aaa.GuissApi.Facade.Models;
using System;
using System.Threading.Tasks;

namespace Refinitiv.Aaa.GuissApi.Facade.Helpers
{
    /// <inheritdoc />
    public class CacheHelper : ICacheHelper
    {
        private readonly IMemcachedResultsClient client;
        private readonly CacheHelperOptions options;
        private readonly ILogger<CacheHelper> logger;


        /// <summary>
        /// Initializes a new instance of the <see cref="CacheHelper"/> class.
        /// </summary>
        /// <param name="options">Options with configuration data.</param>
        /// <param name="client">MemcachedClient for connecting node.</param>
        /// <param name="logger">Logger.</param>
        public CacheHelper(IOptions<CacheHelperOptions> options, IMemcachedResultsClient client, ILogger<CacheHelper> logger)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
                    
            this.options = options.Value;
            this.client = client;
            this.logger = logger;
        }

        /// <inheritdoc />
        public async Task<T> GetValueOrCreateAsync<T>(string key, Func<Task<T>> generator, int? cacheSeconds = null)
        {
            if (!options.Enabled)
            {
                return await generator?.Invoke();
            }

            var result = client.ExecuteGet(key);

            if (result.Success)
            {
                logger.LogDebug($"'{key}' received from memcache");
                return InternalDeserialize<T>((string)result.Value);
            }

            var value = await generator?.Invoke();

            if (value != null)
            {
                Add(key, value, cacheSeconds);
                logger.LogDebug($"'{key}' added to memcache");
            }

            return value;
        }

        /// <inheritdoc />
        public T Get<T>(string key)
        {
            var val = (string)client.ExecuteGet(key).Value;
            return InternalDeserialize<T>(val);
        }

        /// <inheritdoc />
        public bool CreateOrReplace<T>(string key, T value, int? cacheSeconds = null)
        {
            var item = client.ExecuteGet(key);

            if (item.Success)
            {
                return Replace(key, value, cacheSeconds);
            }

            return Add(key, value, cacheSeconds);
        }

        /// <inheritdoc />
        public bool Add<T>(string key, T value, int? cacheSeconds = null)
        {
            IStoreOperationResult result = client.ExecuteStore(StoreMode.Add, key, InternalSerialize(value), GetValidFor(cacheSeconds));
            if (!result.Success)
            {
                logger.LogCritical($"Cannot add '{key}' to memcache: {result.Message}. Exception: {result.Exception}");
            }
            return result.Success;
        }

        /// <inheritdoc />
        public bool Replace<T>(string key, T value, int? cacheSeconds = null)
        {
            IStoreOperationResult result = client.ExecuteStore(StoreMode.Set, key, InternalSerialize(value), GetValidFor(cacheSeconds));
            if (!result.Success)
            {
                logger.LogCritical($"Cannot replace '{key}' in memcache: {result.Message}. Exception: {result.Exception}");
            }
            return result.Success;
        }

        /// <inheritdoc />
        public bool Remove(string key)
        {
            IRemoveOperationResult result = client.ExecuteRemove(key);
            if (!result.Success)
            {
                logger.LogCritical($"Cannot remove '{key}' from memcache: {result.Message}. Exception: {result.Exception}");
            }
            return result.Success;
        }

        private TimeSpan GetValidFor(int? cacheSeconds) 
        {
            return TimeSpan.FromSeconds(cacheSeconds ?? options.DefaultExpirationInSeconds);
        }

        private static string InternalSerialize(object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        private static T InternalDeserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
