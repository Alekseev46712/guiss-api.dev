using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Refinitiv.Aaa.Foundation.ApiClient.Interfaces;
using Refinitiv.Aaa.GuissApi.Facade.Exceptions;
using Refinitiv.Aaa.GuissApi.Facade.Interfaces;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.Configuration;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.UserAttribute;

namespace Refinitiv.Aaa.GuissApi.Facade.Helpers
{
    /// <inheritdoc />
    public class UserApiAttributeAccessor : IUserApiAttributeAccessor
    {
        private readonly IUserHelper userHelper;
        private readonly UserApi userApiSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserApiAttributeAccessor"/> class.
        /// </summary>
        /// <param name="userHelper">User Helper.</param>
        /// <param name="userApiSettings"></param>
        public UserApiAttributeAccessor(
            IUserHelper userHelper,
            IOptions<AppSettings> userApiSettings)
        {
            this.userHelper = userHelper;
            this.userApiSettings = userApiSettings.Value.Services.UserApi;
        }

        /// <inheritdoc />
        public IEnumerable<string> DefaultAttributes => userApiSettings.Attributes;

        /// <inheritdoc />
        public async Task<IEnumerable<UserAttributeDetails>> GetUserAttributesAsync(string userUuid, IEnumerable<string> attributeNames)
        {
            var result = new List<UserAttributeDetails>();

            if (attributeNames == null)
            {
                return result;
            }

            var user = await userHelper.GetUserByUuidAsync(userUuid);

            foreach (var attributeName in attributeNames)
            {
                var property = user.GetType().GetProperty(GetPropertyName(attributeName));

                if (property == null)
                {
                    throw new InvalidAttributeException(attributeName, nameof(UserApi));
                }

                result.Add(new UserAttributeDetails()
                {
                    UserUuid = userUuid,
                    Name = attributeName,
                    Value = property.GetValue(user, null)?.ToString()
                });
            }

            return result;
        }

        private static string GetPropertyName(string attributeName)
        {
            return attributeName.Split('.').Last();
        }
    }
}
