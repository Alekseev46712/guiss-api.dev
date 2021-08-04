using Refinitiv.Aaa.Interfaces.Business;
using System.Collections.Generic;

namespace Refinitiv.Aaa.GuissApi.Interfaces.Models.UserAttribute
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

        /// <summary>
        /// Gets or sets the Names for filtering by them.
        /// </summary>
        /// <value>The Names.</value>
        public IEnumerable<string> Names { get; set; }
    }
}
