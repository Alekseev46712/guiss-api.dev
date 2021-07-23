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

        public async Task<UserAttribute> InsertAsync(UserAttribute item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return await InsertAttributeAsync(item);
        }

        private async Task<UserAttribute> InsertAttributeAsync(UserAttribute item)
        {
            item.UpdatedBy = "123";
            item.UpdatedOn = DateTimeOffset.UtcNow;
            item.SearchName = item.Name.ToLower();
            var dto = item.Map();
           

            var savedGuiss = await userAttributeRepository.SaveAsync(dto).ConfigureAwait(false);
            var newGuiss = savedGuiss.Map();
          

            return newGuiss;
        }
    }
}
