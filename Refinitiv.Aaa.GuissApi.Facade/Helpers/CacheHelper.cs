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

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheHelper"/> class.
        /// </summary>
        /// <param name="hostname">Hostname of the cache node.</param>
        /// <param name="port">Port of the cache node.</param>
        public CacheHelper(string hostname, int port)
        {
            configuration = new MemcachedClientConfiguration();
            configuration.AddServer(hostname, port);
        }

        /// <inheritdoc />
        public async Task<T> GetValueOrCreateAsync<T>(string key, int cacheSeconds, Func<Task<T>> generator)
        {
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
                    client.Store(StoreMode.Add, key, value, TimeSpan.FromSeconds(cacheSeconds));                   
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
        public bool CreateOrReplace<T>(string key, T value, int cacheSeconds)
        {
            using (MemcachedClient client = new MemcachedClient(configuration))
            {
                bool result;

                var item = client.ExecuteGet(key);

                if (item.Success)
                {
                    result = client.Store(StoreMode.Set, key, value, TimeSpan.FromSeconds(cacheSeconds));

                    return result;
                }

                result = client.Store(StoreMode.Add, key, value, TimeSpan.FromSeconds(cacheSeconds));

                return result;
            }
        }

        /// <inheritdoc />
        public bool Add(string key, string value, int cacheSeconds)
        {
            using (MemcachedClient client = new MemcachedClient(configuration))
            {
                bool result = client.Store(StoreMode.Add, key, value, TimeSpan.FromSeconds(cacheSeconds));
                return result;
            }
        }

        /// <inheritdoc />
        public bool Replace(string key, string value, int cacheSeconds)
        {
            using (MemcachedClient client = new MemcachedClient(configuration))
            {
                bool result = client.Store(StoreMode.Set, key, value, TimeSpan.FromSeconds(cacheSeconds));
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
