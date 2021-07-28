using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Refinitiv.Aaa.Foundation.ApiClient.Interfaces;
using Refinitiv.Aaa.GuissApi.Data.Interfaces;
using Refinitiv.Aaa.GuissApi.Data.Models;
using Refinitiv.Aaa.GuissApi.Facade.Interfaces;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.UserAttribute;
using System;
using System.Threading.Tasks;

namespace Refinitiv.Aaa.GuissApi.Facade.Validation
{
    /// <inheritdoc />
    public class UserAttributeValidator : IUserAttributeValidator
    {
        private readonly IUserHelper userHelper;
        private readonly IMapper mapper;
        private readonly IUserAttributeRepository userAttributeRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAttributeValidator"/> class.
        /// </summary>
        /// <param name="userHelper">User Helper.</param>
        /// <param name="userAttributeRepository">User Attribute Repository.</param>
        /// <param name="mapper">Automapper.</param>
        public UserAttributeValidator(IUserHelper userHelper,
            IUserAttributeRepository userAttributeRepository,
            IMapper mapper)
        {
            this.userHelper = userHelper;
            this.userAttributeRepository = userAttributeRepository;
            this.mapper = mapper;
        }

        /// <inheritdoc />
        public Task<IActionResult> ValidateAttributeAsync(UserAttributeDetails userAttributeDetails)
        {
            if (userAttributeDetails == null)
            {
                throw new ArgumentNullException(nameof(userAttributeDetails));
            }

            return InternalValidateAttributeAsync(userAttributeDetails);
        }

        /// <inheritdoc/>
        public Task<IActionResult> ValidateUserUuidAsync(string userUuid)
        {
            var userAttributeDetails = new UserAttributeDetails { UserUuid = userUuid };
            return InternalValidateAttributeAsync(userAttributeDetails);
        }

        /// <inheritdoc/>
        public Task<UserAttribute> ValidatePutRequestAsync(UserAttributeDetails userAttributeDetails)
        {
            if (userAttributeDetails == null)
            {
                throw new ArgumentNullException(nameof(userAttributeDetails));
            }

            return InternalValidatePutRequestAsync(userAttributeDetails);
        }

        private async Task<UserAttribute> InternalValidatePutRequestAsync(UserAttributeDetails userAttributeDetails)
        {
            var exsistingUserAttribute = await userAttributeRepository.FindByUserUuidAndNameAsync(userAttributeDetails.UserUuid, userAttributeDetails.Name);

            if (exsistingUserAttribute == null)
            {
                return null;
            }

            return mapper.Map<UserAttributeDb, UserAttribute>(exsistingUserAttribute);
        }

        private async Task<IActionResult> InternalValidateAttributeAsync(UserAttributeDetails userAttributeDetails)
        {
            var exsistingFromUsersApi = await userHelper.GetUserByUuidAsync(userAttributeDetails.UserUuid);
            if (exsistingFromUsersApi == null)
            {
                return new NotFoundObjectResult(new { Message = "The User is not found" });
            }

            return new AcceptedResult();
        }
    }
}
