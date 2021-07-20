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
    }
}
