using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper.Internal;
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
        public IUserAttributeAccessor GetAccessor(string attributeName)
        {
            foreach (var accessor in externalApiAccessors)
            {
                if(accessor.DefaultAttributes.Contains(attributeName, StringComparer.InvariantCultureIgnoreCase))
                {
                    return accessor;
                }
            }

            return defaultAccessor;
        }

        /// <inheritdoc />
        public Dictionary<IUserAttributeAccessor, List<string>> GetAccessorsWithAttributes(IEnumerable<string> attributeNames)
        {
            var accessors = new Dictionary<IUserAttributeAccessor, List<string>>();

            foreach (var attributeName in attributeNames)
            {
                var accessor = GetAccessor(attributeName);
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
        public Dictionary<IUserAttributeAccessor, List<string>> GetAccessorsWithDefaultAttributes()
        {
            var accessors = new Dictionary<IUserAttributeAccessor, List<string>>();

            externalApiAccessors.ForAll(p => accessors.Add(p, p.DefaultAttributes.ToList()));
            accessors.Add(defaultAccessor, null);

            return accessors;
        }
    }
}
