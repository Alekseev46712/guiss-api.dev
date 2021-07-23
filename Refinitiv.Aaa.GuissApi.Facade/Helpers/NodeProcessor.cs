using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Refinitiv.Aaa.GuissApi.Facade.Helpers
{
    /// <summary>
    /// The json builder for elements with the nested structure of the keys separated by dots in names.
    /// </summary>
    public static class NodeProcessor
    {
        /// <summary>
        /// Builds the json object.
        /// </summary>
        /// <param name="keyValuePairs">Collection of the key-value pairs.</param>
        /// <returns>Json object.</returns>
        public static JObject BuildJsonObject(IReadOnlyCollection<KeyValuePair<string, string>> keyValuePairs)
        {
            return ProcessNode(keyValuePairs);
        }

        private static JObject ProcessNode(IReadOnlyCollection<KeyValuePair<string, string>> relativeChildren)
        {
            var jObject = new JObject();

            // Root properties
            foreach (var pair in relativeChildren.Where(kv => !kv.Key.Contains('.')))
            {
                jObject[pair.Key] = pair.Value;
            }

            var groupedProperties = relativeChildren
                .Where(kv => kv.Key.Contains('.'))
                .GroupBy(kv => kv.Key.Split('.')[0])
                .Select(group => new NodeData
                {
                    Key = group.Key,
                    RelativeChildren = group
                        .Select(el => new KeyValuePair<string, string>(
                            el.Key.Substring(group.Key.Length + 1),
                            el.Value))
                        .ToList(),
                });

            foreach (var groupedProperty in groupedProperties)
            {
                jObject[groupedProperty.Key] = ProcessNode(groupedProperty.RelativeChildren);
            }

            return jObject;
        }
    }
}
