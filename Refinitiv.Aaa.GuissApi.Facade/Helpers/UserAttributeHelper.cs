using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Refinitiv.Aaa.Foundation.ApiClient.Helpers;
using Refinitiv.Aaa.GuissApi.Data.Interfaces;
using Refinitiv.Aaa.GuissApi.Facade.Extensions;
using Refinitiv.Aaa.GuissApi.Facade.Interfaces;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.UserAttribute;

namespace Refinitiv.Aaa.GuissApi.Facade.Helpers
{
    /// <inheritdoc />
    public class UserAttributeHelper : IUserAttributeHelper
    {
        private readonly IUserAttributeRepository userAttributeRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppAccountHelper"/> class.
        /// </summary>
        /// <param name="userAttributeRepository">Repository used to access the data.</param>
        public UserAttributeHelper(
            IUserAttributeRepository userAttributeRepository)
        {
            this.userAttributeRepository = userAttributeRepository;
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

        public async Task<UserAttribute> InsertAsync(UserAttribute userAttribute)
        {
            if (userAttribute == null)
            {
                throw new ArgumentNullException(nameof(userAttribute));
            }

            return await InsertAttributeAsync(userAttribute);
        }

        private async Task<UserAttribute> InsertAttributeAsync(UserAttribute userAttribute)
        {
           
            var userAttributeDb = userAttribute.Map();
           

            var savedGuiss = await userAttributeRepository.SaveAsync(userAttributeDb).ConfigureAwait(false);
            var newAttribute = savedGuiss.Map();
          

            return newAttribute;
        }

        public Task<UserAttribute> UpdateAsync(UserAttribute userAttribute)
        {
            if (userAttribute == null)
            {
                throw new ArgumentNullException(nameof(userAttribute));
            }

            return UpdateAttributeAsync(userAttribute);
        }

        private async Task<UserAttribute> UpdateAttributeAsync(UserAttribute userAttribute)
        {
            var userAttributeDb = userAttribute.Map();

            var savedUserAttribute = await userAttributeRepository.SaveAsync(userAttributeDb).ConfigureAwait(false);
            var newAttribute = savedUserAttribute.Map();
           

            return newAttribute;
           
        }
    }
}
