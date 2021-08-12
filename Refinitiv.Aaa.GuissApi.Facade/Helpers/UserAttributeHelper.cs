using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Newtonsoft.Json.Linq;
using Refinitiv.Aaa.Foundation.ApiClient.Helpers;
using Refinitiv.Aaa.GuissApi.Data.Interfaces;
using Refinitiv.Aaa.GuissApi.Data.Models;
using Refinitiv.Aaa.GuissApi.Facade.Interfaces;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.UserAttribute;
using Refinitiv.Aaa.Interfaces.Headers;

namespace Refinitiv.Aaa.GuissApi.Facade.Helpers
{
    /// <inheritdoc />
    public class UserAttributeHelper : IUserAttributeHelper
    {
        private readonly IUserAttributeRepository userAttributeRepository;
        private readonly IMapper mapper;
        private readonly IAaaRequestHeaders aaaRequestHeaders;
        private readonly IUserAttributeProvider userAttributeProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppAccountHelper"/> class.
        /// </summary>
        /// <param name="userAttributeRepository">Repository used to access the data.</param>
        /// <param name="mapper">Automapper.</param>
        /// <param name="aaaRequestHeaders">Request headers.</param>
        /// <param name="userAttributeProvider"></param>
        public UserAttributeHelper(
            IUserAttributeRepository userAttributeRepository,
            IMapper mapper,
            IAaaRequestHeaders aaaRequestHeaders,
            IUserAttributeProvider userAttributeProvider)
        {
            this.userAttributeRepository = userAttributeRepository;
            this.mapper = mapper;
            this.aaaRequestHeaders = aaaRequestHeaders;
            this.userAttributeProvider = userAttributeProvider;
        }

        /// <inheritdoc />
        public async Task<JObject> GetAllByUserUuidAsync(string userUuid)
        {
            var userAttributes = await userAttributeProvider.GetUserAttributesAsync(userUuid);
            var result =  GetJsonObject(userAttributes);

            return result;
        }

        /// <inheritdoc />
        public Task<JObject> GetAttributesByUserUuidAsync(string userUuid, string attributes)
        {
            if (attributes == null)
            {
                throw new ArgumentNullException(nameof(attributes));
            }

            return PerformGetAttributesByUserUuidAsync();

            async Task<JObject> PerformGetAttributesByUserUuidAsync()
            {
                var attributesList = attributes.ToLower(CultureInfo.CurrentCulture).Split(',').ToList();

                var userAttributes = await userAttributeProvider.GetUserAttributesAsync(userUuid, attributesList);
                var result = GetJsonObject(userAttributes);

                return result;
            }
        }

        /// <inheritdoc />
        public Task<JObject> GetAttributesByUserNamespacesAndUuidAsync(string userUuid, string namespaces)
        {
            if (namespaces == null)
            {
                throw new ArgumentNullException(nameof(namespaces));
            }

            return PerformGetAttributesByUserNamespacesAndUuidAsync();

            async Task<JObject> PerformGetAttributesByUserNamespacesAndUuidAsync()
            {
                var namespacesList = namespaces.ToLower(CultureInfo.CurrentCulture).Split(',').ToList();
                var filter = new UserAttributeFilter
                {
                    UserUuid = userUuid,
                    Namespaces = namespacesList
                };

                var userAttributesDb = await userAttributeRepository.SearchAsync(filter);
                var userAttributesDetails = mapper.Map<IEnumerable<UserAttributeDetails>>(userAttributesDb);
                var result = GetJsonObject(userAttributesDetails);

                return result;
            }
        }

        /// <inheritdoc />
        public async Task<UserAttributeDb> FindByUserUuidAndNameAsync(string uuid, string name)
        {
            return await userAttributeRepository.FindByUserUuidAndNameAsync(uuid, name);
        }

        /// <inheritdoc />
        public async Task DeleteUserAttributeAsync(string id, string name)
        {
            await userAttributeRepository.DeleteAsync(id, name);
        }

        /// <inheritdoc />
        public Task<UserAttribute> InsertAsync(UserAttributeDetails userAttributeDetails)
        {
            if (userAttributeDetails == null)
            {
                throw new ArgumentNullException(nameof(userAttributeDetails));
            }

            return InsertAttributeAsync(userAttributeDetails);
        }

        /// <inheritdoc />
        public Task<UserAttribute> UpdateAsync(UserAttribute userAttribute, string value)
        {
            if (userAttribute == null)
            {
                throw new ArgumentNullException(nameof(userAttribute));
            }

            return UpdateAttributeAsync(userAttribute, value);
        }

        private async Task<UserAttribute> InsertAttributeAsync(UserAttributeDetails userAttributeDetails)
        {
            var userAttribute = CreateUserAttributeObject(userAttributeDetails);

            var userAttributeDb = mapper.Map<UserAttribute, UserAttributeDb>(userAttribute);

            var savedAttribute = await userAttributeRepository.SaveAsync(userAttributeDb);

            var newAttribute = mapper.Map<UserAttributeDb, UserAttribute>(savedAttribute);

            return newAttribute;
        }

        private async Task<UserAttribute> UpdateAttributeAsync(UserAttribute userAttribute, string value)
        {
            var userAttributeDb = mapper.Map<UserAttribute, UserAttributeDb>(userAttribute);

            userAttributeDb.Value = value;
            userAttributeDb.UpdatedBy = aaaRequestHeaders.RefinitivUuid;
            userAttributeDb.UpdatedOn = DateTime.UtcNow;

            var savedUserAttribute = await userAttributeRepository.SaveAsync(userAttributeDb);

            var newAttribute = mapper.Map<UserAttributeDb, UserAttribute>(savedUserAttribute);

            return newAttribute;
        }

        private UserAttribute CreateUserAttributeObject(UserAttributeDetails userAttributeDetails)
        {
            if (userAttributeDetails == null)
            {
                throw new ArgumentNullException(nameof(userAttributeDetails));
            }
            return PerformCreateUserAttributeObject(userAttributeDetails);
        }

        private UserAttribute PerformCreateUserAttributeObject(UserAttributeDetails userAttributeDetails)
        {
            var userAttribute = mapper.Map<UserAttributeDetails, UserAttribute>(userAttributeDetails);
            userAttribute.UpdatedOn = DateTime.UtcNow;
            userAttribute.UpdatedBy = aaaRequestHeaders.RefinitivUuid;

            return userAttribute;
        }

        private JObject GetJsonObject(IEnumerable<UserAttributeDetails> userAttributeDetails)
        {
            var dataToParse = userAttributeDetails
                .Select(x => new KeyValuePair<string, string>(x.Name, x.Value))
                .ToList();

            var result = NodeProcessor.BuildJsonObject(dataToParse);

            return result;
        }
    }
}
