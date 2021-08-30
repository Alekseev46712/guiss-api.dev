using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;
using Enyim.Caching.Memcached.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Refinitiv.Aaa.GuissApi.Facade.Interfaces;
using Refinitiv.Aaa.GuissApi.Facade.Models;
using System;
using System.Collections.Generic;
using System.Text;
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
        public CacheHelper(IOptions<CacheHelperOptions> options, IMemcachedResultsClient client, ILogger<CacheHelper> logger)
        {
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
                return InternalDeserialize<T>((string)result.Value);
            }

            var value = await generator?.Invoke();

            if (value != null)
            {
                var storeResult = client.ExecuteStore(StoreMode.Add, key, JsonConvert.SerializeObject(value), GetValidFor(cacheSeconds));
                if (!storeResult.Success)
                {
                    logger.LogError($"{options.Hostname} {options.Port}");
                    throw new ArgumentException(storeResult.Message);
                }
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
            IStoreOperationResult result;

            var item = client.ExecuteGet(key);

            if (item.Success)
            {
                result = client.ExecuteStore(StoreMode.Set, key, JsonConvert.SerializeObject(value), GetValidFor(cacheSeconds));

                return result.Success;
            }

            result = client.ExecuteStore(StoreMode.Add, key, JsonConvert.SerializeObject(value), GetValidFor(cacheSeconds));

            return result.Success;
        }

        /// <inheritdoc />
        public bool Add<T>(string key, T value, int? cacheSeconds = null)
        {
            IStoreOperationResult result = client.ExecuteStore(StoreMode.Add, key, JsonConvert.SerializeObject(value), GetValidFor(cacheSeconds));
            return result.Success;
        }

        /// <inheritdoc />
        public bool Replace<T>(string key, T value, int? cacheSeconds = null)
        {
            IStoreOperationResult result = client.ExecuteStore(StoreMode.Set, key, JsonConvert.SerializeObject(value), GetValidFor(cacheSeconds));
            return result.Success;
        }

        /// <inheritdoc />
        public bool Remove(string key)
        {
            IRemoveOperationResult result = client.ExecuteRemove(key);
            return result.Success;
        }

        private TimeSpan GetValidFor(int? cacheSeconds) 
        {
            return TimeSpan.FromSeconds(cacheSeconds ?? options.DefaultExpirationInSeconds);
        }

        private T InternalDeserialize<T>(string json)
        {
            if (typeof(T) == typeof(string))
            {
                return (T)Convert.ChangeType(json, typeof(T));
            }

            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
