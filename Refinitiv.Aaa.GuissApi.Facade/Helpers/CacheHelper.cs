using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;
using Refinitiv.Aaa.GuissApi.Facade.Interfaces; 
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Refinitiv.Aaa.GuissApi.Facade.Helpers
{
    /// <inheritdoc />
    public class CacheHelper : ICacheHelper
    {
        private readonly MemcachedClientConfiguration configuration;
        private readonly int DefaultExpirationInSeconds;
        private readonly bool Enabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheHelper"/> class.
        /// </summary>
        /// <param name="hostname">Hostname of the cache node.</param>
        /// <param name="port">Port of the cache node.</param>
        /// <param name="DefaultExpirationInSeconds">Time that cache lives.</param>
        /// <param name="Enabled">Enable or disable caching.</param>
        public CacheHelper(string hostname, int port, int DefaultExpirationInSeconds, bool Enabled)
        {
            configuration = new MemcachedClientConfiguration();
            configuration.AddServer(hostname, port);
            this.DefaultExpirationInSeconds = DefaultExpirationInSeconds;
            this.Enabled = Enabled;
        }

        /// <inheritdoc />
        public async Task<T> GetValueOrCreateAsync<T>(string key, Func<Task<T>> generator, int? cacheSeconds = null)
        {
            if (!Enabled)
            {
                return await generator?.Invoke();
            }
            using (MemcachedClient client = new MemcachedClient(configuration))
            {
                var result = client.ExecuteGet(key);

                if (result.Success)
                {
                    return (T)result.Value;
                }

                var value = await generator?.Invoke();

                if (value != null)
                {
                    client.Store(StoreMode.Add, key, value, TimeSpan.FromSeconds(cacheSeconds ?? DefaultExpirationInSeconds));                   
                }

                return value;
            }
        }

        /// <inheritdoc />
        public T Get<T>(string key)
        {
            using (MemcachedClient client = new MemcachedClient(configuration))
            {
                return client.Get<T>(key);
            }
        }

        /// <inheritdoc />
        public bool CreateOrReplace<T>(string key, T value, int? cacheSeconds = null)
        {
            using (MemcachedClient client = new MemcachedClient(configuration))
            {
                bool result;

                var item = client.ExecuteGet(key);

                if (item.Success)
                {
                    result = client.Store(StoreMode.Set, key, value, TimeSpan.FromSeconds(cacheSeconds ?? DefaultExpirationInSeconds));

                    return result;
                }

                result = client.Store(StoreMode.Add, key, value, TimeSpan.FromSeconds(cacheSeconds ?? DefaultExpirationInSeconds));

                return result;
            }
        }

        /// <inheritdoc />
        public bool Add<T>(string key, T value, int? cacheSeconds = null)
        {
            using (MemcachedClient client = new MemcachedClient(configuration))
            {
                bool result = client.Store(StoreMode.Add, key, value, TimeSpan.FromSeconds(cacheSeconds ?? DefaultExpirationInSeconds));
                return result;
            }
        }

        /// <inheritdoc />
        public bool Replace<T>(string key, T value, int? cacheSeconds = null)
        {
            using (MemcachedClient client = new MemcachedClient(configuration))
            {
                bool result = client.Store(StoreMode.Set, key, value, TimeSpan.FromSeconds(cacheSeconds ?? DefaultExpirationInSeconds));
                return result;
            }
        }

        /// <inheritdoc />
        public bool Remove(string key)
        {
            using (MemcachedClient client = new MemcachedClient(configuration))
            {
                bool result = client.Remove(key);
                return result;
            }
        }
    }
}
