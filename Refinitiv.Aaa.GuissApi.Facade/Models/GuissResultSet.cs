using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Refinitiv.Aaa.Interfaces.Business;

[assembly: InternalsVisibleTo("Refinitiv.Aaa.GuissApi.Tests")]
namespace Refinitiv.Aaa.GuissApi.Facade.Models
{
    /// <summary>
    /// A subset of Guiss items.
    /// </summary>
    internal class GuissResultSet : IResultSet<Guiss>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GuissResultSet"/> class.
        /// </summary>
        public GuissResultSet()
        {
            Items = new List<Guiss>();
        }

        /// <summary>
        /// Gets or sets a link to the previous page of search results.
        /// </summary>
        /// <value>A link to the previous page of search results.</value>
        public string Previous { get; set; }

        /// <summary>
        /// Gets or sets a link to the next page of search results.
        /// </summary>
        /// <value>A link to the next page of search results.</value>
        public string Next { get; set; }

        /// <summary>
        /// Gets or sets subset of Guiss items.
        /// </summary>
        /// <value>A Collection of Guiss Items.</value>
        public IEnumerable<Guiss> Items { get; set; }
    }
}
