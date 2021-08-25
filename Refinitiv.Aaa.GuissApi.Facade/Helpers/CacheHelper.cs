using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;
using Enyim.Caching.Memcached.Results;
using Microsoft.Extensions.Options;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheHelper"/> class.
        /// </summary>
        /// <param name="options">Options with configuration data.</param>
        /// <param name="client">MemcachedClient for connecting node.</param>
        public CacheHelper(IOptions<CacheHelperOptions> options, IMemcachedResultsClient client)
        {
            this.options = options.Value;
            this.client = client;
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
                return (T)result.Value;
            }

            var value = await generator?.Invoke();

            if (value != null)
            {
                client.ExecuteStore(StoreMode.Add, key, value, TimeSpan.FromSeconds(cacheSeconds ?? options.DefaultExpirationInSeconds));
            }

            return value;
        }

        /// <inheritdoc />
        public T Get<T>(string key)
        {
            return client.ExecuteGet<T>(key).Value;
        }

        /// <inheritdoc />
        public bool CreateOrReplace<T>(string key, T value, int? cacheSeconds = null)
        {
            IStoreOperationResult result;

            var item = client.ExecuteGet(key);

            if (item.Success)
            {
                result = client.ExecuteStore(StoreMode.Set, key, value, TimeSpan.FromSeconds(cacheSeconds ?? options.DefaultExpirationInSeconds));

                return result.Success;
            }

            result = client.ExecuteStore(StoreMode.Add, key, value, TimeSpan.FromSeconds(cacheSeconds ?? options.DefaultExpirationInSeconds));

            return result.Success;
        }

        /// <inheritdoc />
        public bool Add<T>(string key, T value, int? cacheSeconds = null)
        {
            IStoreOperationResult result = client.ExecuteStore(StoreMode.Add, key, value, TimeSpan.FromSeconds(cacheSeconds ?? options.DefaultExpirationInSeconds));
            return result.Success;
        }

        /// <inheritdoc />
        public bool Replace<T>(string key, T value, int? cacheSeconds = null)
        {
            IStoreOperationResult result = client.ExecuteStore(StoreMode.Set, key, value, TimeSpan.FromSeconds(cacheSeconds ?? options.DefaultExpirationInSeconds));
            return result.Success;
        }

        /// <inheritdoc />
        public bool Remove(string key)
        {
            IRemoveOperationResult result = client.ExecuteRemove(key);
            return result.Success;
        }
    }
}
