using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;
using System;
using System.Collections.Generic;
using System.Text;

namespace Refinitiv.Aaa.GuissApi.Facade.Helpers
{
    public class CacheHelper
    {
        MemcachedClientConfiguration local;

        public CacheHelper(string hostname, int port)
        {
            //config = new ElastiCacheClusterConfig(hostname, port);
            local = new MemcachedClientConfiguration();
            local.AddServer(hostname, port);
        }

        public string CreateSomeData()
        {
            using (MemcachedClient client = new MemcachedClient(local))
            {

                bool results = client.Store(StoreMode.Add, "custom_key1", "custom_value");

                return results.ToString();         
            }
        }





    }
}
