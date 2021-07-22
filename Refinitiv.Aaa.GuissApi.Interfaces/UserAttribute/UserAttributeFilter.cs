using Refinitiv.Aaa.Interfaces.Business;

namespace Refinitiv.Aaa.GuissApi.Interfaces.UserAttribute
{
    public class UserAttributeFilter : IFilter
    {
        /// <summary>
        /// Gets or sets the UserUuid for filtering by it.
        /// </summary>
        /// <value>The UserUuid.</value>
        public string UserUuid { get; set; }

        /// <summary>
        /// Gets or sets the Name for filtering by it.
        /// </summary>
        /// <value>The Name.</value>
        public string Name { get; set; }
    }
}
