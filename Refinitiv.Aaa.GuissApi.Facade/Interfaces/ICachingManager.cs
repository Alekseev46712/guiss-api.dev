using Newtonsoft.Json.Linq;

namespace Refinitiv.Aaa.GuissApi.Facade.Interfaces
{
    /// <summary>
    /// The caching manager.
    /// </summary>
    public interface ICachingManager
    {
        /// <summary>
        /// Gets the user data related to specified API name.
        /// </summary>
        /// <param name="apiName">The name of the API the data related to.</param>
        JObject GetUserData(string apiName);

        /// <summary>
        /// Sets the user data to specified API name.
        /// </summary>
        /// <param name="apiName">The name of the API the data related to.</param>
        /// <param name="value">The user data.</param>
        void SetUserData(string apiName, JObject value);
    }
}
