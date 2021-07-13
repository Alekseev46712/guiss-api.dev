using System.Security.Principal;

namespace Refinitiv.Aaa.GuissApi.Facade.Interfaces
{
    /// <summary>
    /// Interface for ILogHelper.
    /// </summary>
    public interface ILogHelper
    {
        /// <summary>
        /// Method which returns the RequesterId for a given user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>RequesterId string for a given user.</returns>
        string GetRequesterId(IPrincipal user);
    }
}
