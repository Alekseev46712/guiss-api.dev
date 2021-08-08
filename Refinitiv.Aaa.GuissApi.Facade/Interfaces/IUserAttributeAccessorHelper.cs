using System.Collections.Generic;

namespace Refinitiv.Aaa.GuissApi.Facade.Interfaces
{
    /// <summary>
    /// Provides methods for interacting with user attribute accessors.
    /// </summary>
    public interface IUserAttributeAccessorHelper
    {
        /// <summary>
        /// Gets accessor by attribute name.
        /// </summary>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        IUserAttributeAccessor GetAccessor(string attributeName);

        /// <summary>
        /// Gets accessors with appropriate attributes from the giving list.
        /// </summary>
        /// <param name="attributeNames"></param>
        /// <returns></returns>
        Dictionary<IUserAttributeAccessor, IEnumerable<string>> GetAccessorsWithAttributes(
            IEnumerable<string> attributeNames);

        /// <summary>
        /// Gets accessors with all the appropriate attributes.
        /// </summary>
        /// <returns></returns>
        Dictionary<IUserAttributeAccessor, IEnumerable<string>> GetAccessorsWithDefaultAttributes();
    }
}
