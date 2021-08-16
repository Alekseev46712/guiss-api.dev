using System.Collections.Generic;
using System.Threading.Tasks;
using Refinitiv.Aaa.GuissApi.Facade.Interfaces;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.UserAttribute;

namespace Refinitiv.Aaa.GuissApi.Facade.Helpers
{
    /// <inheritdoc />
    public class UserAttributeProvider : IUserAttributeProvider
    {
        private readonly IUserAttributeAccessorHelper accessorsHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAttributeProvider"/> class.
        /// </summary>l
        /// <param name="accessorsHelper"></param>
        public UserAttributeProvider(IUserAttributeAccessorHelper accessorsHelper)
        {
            this.accessorsHelper = accessorsHelper;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<UserAttributeDetails>> GetUserAttributesAsync(string userUuid)
        {
            var accessorsWithAttributes = accessorsHelper.GetAccessorsWithDefaultAttributes();
            var attributeDetails = await GetUserAttributesAsync(userUuid, accessorsWithAttributes);

            return attributeDetails;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<UserAttributeDetails>> GetUserAttributesAsync(
            string userUuid,
            IEnumerable<string> attributeNames)
        {
            var accessorsWithAttributes = accessorsHelper.GetAccessorsWithAttributes(attributeNames);
            var attributeDetails = await GetUserAttributesAsync(userUuid, accessorsWithAttributes);

            return attributeDetails;
        }

        private async Task<IEnumerable<UserAttributeDetails>> GetUserAttributesAsync(
            string userUuid,
            Dictionary<IUserAttributeAccessor, List<string>> accessorsWithAttributes)
        {
            var result = new List<UserAttributeDetails>();

            foreach (var accessor in accessorsWithAttributes)
            {
                var attributesDetails = await accessor.Key.GetUserAttributesAsync(userUuid, accessor.Value);
                result.AddRange(attributesDetails);
            }

            return result;
        }
    }
}
