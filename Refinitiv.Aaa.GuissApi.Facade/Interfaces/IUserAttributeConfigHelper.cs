using Refinitiv.Aaa.GuissApi.Interfaces.Models.Configuration;
using System.Threading.Tasks;

namespace Refinitiv.Aaa.GuissApi.Facade.Interfaces
{
    /// <summary>
    /// Helps to work with user attribute config file.
    /// </summary>
    public interface IUserAttributeConfigHelper
    {
        /// <summary>
        /// Gets UserAttributeApiConfig from Parameter Store.
        /// </summary>
        /// <param name="apiName">Name of the API.</param>
        Task<UserAttributeApiConfig> GetUserAttributeApiConfig(string apiName);
    }
}
