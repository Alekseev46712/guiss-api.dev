using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Refinitiv.Aaa.GuissApi.Facade.Interfaces
{
    /// <summary>
    /// Validates requests
    /// </summary>
    public interface ICacheHelper
    {
        /// <summary>
        /// Added new item to cache, or retrieve item if there is value for the key
        /// </summary>
        /// <param name="key">Key to add.</param>
        /// <param name="cacheSeconds">Seconds for which the item will be stored.</param>
        /// <param name="generator">Method that returns value.</param>
        /// <returns>Value if it already exist, true if value was added successfully, false otherwise.</returns>
        Task<T> GetValueOrCreateAsync<T>(string key, int cacheSeconds, Func<Task<T>> generator);

        /// <summary>
        /// Added new item to cache, or replace for the key if key already exist
        /// </summary>
        /// <param name="key">Key to add.</param>
        /// <param name="cacheSeconds">Seconds for which the item will be stored.</param>
        /// <param name="value">Value to add.</param>
        /// <returns>true if one of operations(add,set) was successful, false otherwise.</returns>
        bool CreateOrReplace<T>(string key, T value, int cacheSeconds);


        /// <summary>
        /// Added new item to cache
        /// </summary>
        /// <param name="key">Key to add.</param>
        /// <param name="cacheSeconds">Seconds for which the item will be stored.</param>
        /// <param name="value">Value to add.</param>
        /// <returns>true if operations was successful, false otherwise.</returns>
        bool Add<T>(string key, T value, int cacheSeconds);

        /// <summary>
        /// Replace value for the key
        /// </summary>
        /// <param name="key">Key of item.</param>
        /// <param name="cacheSeconds">Seconds for which the item will be stored.</param>
        /// <param name="value">Value replace.</param>
        /// <returns>true if operation was successful, false otherwise.</returns>
        bool Replace<T>(string key, T value, int cacheSeconds);

        /// <summary>
        /// Remove item
        /// </summary>
        /// <param name="key">Key of item to remove.</param>
        /// <returns>true if operation was successful, false otherwise.</returns>
        bool Remove(string key);

        /// <summary>
        /// Returns value for the key
        /// </summary>
        /// <param name="key">Key of item to get.</param>
        /// <returns>Value for the key.</returns>
        T Get<T>(string key);       
    }
}
