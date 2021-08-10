using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Refinitiv.Aaa.GuissApi.Data.Interfaces;
using Refinitiv.Aaa.GuissApi.Facade.Interfaces;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.UserAttribute;

namespace Refinitiv.Aaa.GuissApi.Facade.Helpers
{
    /// <inheritdoc />
    public class DynamoDbUserAttributeAccessor : IUserAttributeAccessor
    {
        private readonly IUserAttributeRepository userAttributeRepository;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamoDbUserAttributeAccessor"/> class.
        /// </summary>
        /// <param name="userAttributeRepository">Repository used to access the data.</param>
        /// <param name="mapper">Automapper.</param>
        public DynamoDbUserAttributeAccessor(
            IUserAttributeRepository userAttributeRepository,
            IMapper mapper)
        {
            this.userAttributeRepository = userAttributeRepository;
            this.mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<UserAttributeDetails>> GetUserAttributesAsync(string userUuid, IEnumerable<string> attributeNames)
        {
            var filter = new UserAttributeFilter
            {
                UserUuid = userUuid,
                Names = attributeNames
            };

            var userAttributesDb = await userAttributeRepository.SearchAsync(filter);

            var userAttributesDetails = mapper.Map<IEnumerable<UserAttributeDetails>>(userAttributesDb);
            return userAttributesDetails;
        }
    }
}
