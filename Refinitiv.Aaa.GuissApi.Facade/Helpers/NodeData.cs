using System.Collections.Generic;

namespace Refinitiv.Aaa.GuissApi.Facade.Helpers
{
    /// <summary>
    /// Represents the node data for json object.
    /// </summary>
    public struct NodeData
    {
        /// <summary>
        /// The key of the node.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The children of the node.
        /// </summary>
        public IReadOnlyCollection<KeyValuePair<string, string>> RelativeChildren { get; set; }
    }
}
