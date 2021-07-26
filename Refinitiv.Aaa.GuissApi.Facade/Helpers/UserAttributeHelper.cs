using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Newtonsoft.Json.Linq;
using Refinitiv.Aaa.Foundation.ApiClient.Helpers;
using Refinitiv.Aaa.GuissApi.Data.Interfaces;
using Refinitiv.Aaa.GuissApi.Data.Models;
using Refinitiv.Aaa.GuissApi.Facade.Extensions;
using Refinitiv.Aaa.GuissApi.Facade.Interfaces;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.UserAttribute;

namespace Refinitiv.Aaa.GuissApi.Facade.Helpers
{
    /// <inheritdoc />
    public class UserAttributeHelper : IUserAttributeHelper
    {
        private readonly IUserAttributeRepository userAttributeRepository;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppAccountHelper"/> class.
        /// </summary>
        /// <param name="userAttributeRepository">Repository used to access the data.</param>
        /// <param name="mapper">Automapper.</param>
        public UserAttributeHelper(
            IUserAttributeRepository userAttributeRepository,
            IMapper mapper)
        {
            this.userAttributeRepository = userAttributeRepository;
            this.mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<JObject> GetAllByUserUuidAsync(string userUuid)
        {
            var filter = new UserAttributeFilter
            {
                UserUuid = userUuid
            };

            var items = await userAttributeRepository.SearchAsync(filter);

            var dataToParse = items.
                Select(x => new KeyValuePair<string, string>(x.Name, x.Value))
                .ToList();

            var result = NodeProcessor.BuildJsonObject(dataToParse);

            return result;
        }

        /// <inheritdoc />
        public async Task<UserAttribute> InsertAsync(UserAttribute userAttribute)
        {
            if (userAttribute == null)
            {
                throw new ArgumentNullException(nameof(userAttribute));
            }

            return await InsertAttributeAsync(userAttribute);
        }

        /// <inheritdoc />
        private async Task<UserAttribute> InsertAttributeAsync(UserAttribute userAttribute)
        {     
            var userAttributeDb = mapper.Map<UserAttribute, UserAttributeDb>(userAttribute);

            userAttributeDb.Name = userAttribute.Name.ToLower();

            var savedAttribute = await userAttributeRepository.SaveAsync(userAttributeDb);

            var newAttribute = mapper.Map<UserAttributeDb, UserAttribute>(savedAttribute);

            return newAttribute;
        }

        /// <inheritdoc />
        public Task<UserAttribute> UpdateAsync(UserAttribute userAttribute)
        {
            if (userAttribute == null)
            {
                throw new ArgumentNullException(nameof(userAttribute));
            }

            return UpdateAttributeAsync(userAttribute);
        }

        /// <inheritdoc />
        private async Task<UserAttribute> UpdateAttributeAsync(UserAttribute userAttribute)
        {
            var userAttributeDb = mapper.Map<UserAttribute, UserAttributeDb>(userAttribute);

            var savedUserAttribute = await userAttributeRepository.SaveAsync(userAttributeDb);

            var newAttribute = mapper.Map<UserAttributeDb, UserAttribute>(savedUserAttribute);

            return newAttribute;           
        }
    }
}
