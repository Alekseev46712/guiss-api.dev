using Refinitiv.Aaa.GuissApi.Interfaces.Models.Configuration;

namespace Refinitiv.Aaa.GuissApi.Facade.Interfaces
{
    /// <summary>
    /// Helps to work with user attribute config file.
    /// </summary>
    public interface IUserAttributeConfigHelper
    {
        /// <summary>
        /// Gets UserAttributeApiConfig.
        /// </summary>
        /// <param name="apiName">Name of the API.</param>
        UserAttributeApiConfig GetUserAttributeApiConfig(string apiName);
    }
}
