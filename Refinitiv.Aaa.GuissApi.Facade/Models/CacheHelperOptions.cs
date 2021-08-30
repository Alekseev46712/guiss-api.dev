using System;
using System.Collections.Generic;
using System.Text;

namespace Refinitiv.Aaa.GuissApi.Facade.Models
{
    /// <summary>
    /// Represents the options for CacheHelper
    /// </summary>
    public class CacheHelperOptions
    {
        /// <summary>
        /// Cache expiration time in seconds by default
        /// </summary>
        public int DefaultExpirationInSeconds { get; set; }

        /// <summary>
        /// Flag that represents if caching is enabled 
        /// </summary>
        public bool Enabled { get; set; }
    }
}
