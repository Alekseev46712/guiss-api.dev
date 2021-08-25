using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Refinitiv.Aaa.GuissApi.Facade.Interfaces;

namespace Refinitiv.Aaa.GuissApi.Facade.Helpers
{
    /// <inheritdoc />
    public class UserAttributeAccessorHelper : IUserAttributeAccessorHelper
    {
        private readonly DynamoDbUserAttributeAccessor defaultAccessor;
        private readonly ICollection<IExternalUserAttributeAccessor> externalApiAccessors;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAttributeAccessorHelper"/> class.
        /// </summary>
        /// <param name="userApiAttributeAccessor"></param>
        /// <param name="dynamoDbUserAttributeAccessor"></param>
        public UserAttributeAccessorHelper(
            UserApiAttributeAccessor userApiAttributeAccessor,
            DynamoDbUserAttributeAccessor dynamoDbUserAttributeAccessor)
        {
            defaultAccessor = dynamoDbUserAttributeAccessor;
            externalApiAccessors = new List<IExternalUserAttributeAccessor>()
            {
                userApiAttributeAccessor
            };
        }

        /// <inheritdoc />
        public async Task<IUserAttributeAccessor> GetAccessorAsync(string attributeName)
        {
            foreach (var accessor in externalApiAccessors)
            {
                var defaultAttributes = await accessor.GetDefaultAttributesAsync();
                if (defaultAttributes.Contains(attributeName, StringComparer.InvariantCultureIgnoreCase))
                {
                    return accessor;
                }
            }

            return defaultAccessor;
        }

        /// <inheritdoc />
        public async Task<Dictionary<IUserAttributeAccessor, List<string>>> GetAccessorsWithAttributesAsync(IEnumerable<string> attributeNames)
        {
            var accessors = new Dictionary<IUserAttributeAccessor, List<string>>();

            foreach (var attributeName in attributeNames)
            {
                var accessor = await GetAccessorAsync(attributeName);
                if(accessors.TryGetValue(accessor, out var names))
                {
                    names.Add(attributeName);
                }
                else
                {
                    accessors.Add(accessor, new List<string>{attributeName});
                }
            }

            return accessors;
        }

        /// <inheritdoc />
        public async Task<Dictionary<IUserAttributeAccessor, List<string>>> GetAccessorsWithDefaultAttributesAsync()
        {
            var accessors = new Dictionary<IUserAttributeAccessor, List<string>>();

            foreach (var accessor in externalApiAccessors)
            {
                var defaultAttributes = await accessor.GetDefaultAttributesAsync();
                accessors.Add(accessor, defaultAttributes.ToList());
            }
            accessors.Add(defaultAccessor, null);

            return accessors;
        }
    }
}
