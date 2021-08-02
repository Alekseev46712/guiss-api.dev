using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Refinitiv.Aaa.Foundation.ApiClient.Interfaces;
using Refinitiv.Aaa.GuissApi.Data.Interfaces;
using Refinitiv.Aaa.GuissApi.Data.Models;
using Refinitiv.Aaa.GuissApi.Facade.Interfaces;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.UserAttribute;
using System;
using System.Linq;
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

            return InternalValidateUserUuidAsync(userAttributeDetails.UserUuid);
        }

        /// <inheritdoc/>
        public async Task<IActionResult> ValidateUserUuidAsync(string userUuid)
        {
            if (String.IsNullOrEmpty(userUuid))
            {
                return new BadRequestObjectResult(new { Message = "The User is null or empty" });
            }

            return await InternalValidateUserUuidAsync(userUuid);
        }

        /// <inheritdoc/>
        public IActionResult ValidateAttributesString(string attributes)
        {
            if (String.IsNullOrEmpty(attributes) || !attributes.Split(',').Any())
            {
                return new BadRequestObjectResult(new { Message = "The attributes string is null or empty" });
            }
            return new AcceptedResult();
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

        /// <inheritdoc/>
        public async Task<IActionResult> ValidateUserAttributesAsync(string userUuid, string name)
        {
            var exsistingFromUsersApi = await userAttributeRepository.FindByUserUuidAndNameAsync(userUuid, name);
            if (exsistingFromUsersApi == null)
            {
                return new NotFoundObjectResult(new { Message = "Attribute does not exist" });
            }

            return new AcceptedResult();
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

        private async Task<IActionResult> InternalValidateUserUuidAsync(string userUuid)
        {
            var exsistingFromUsersApi = await userHelper.GetUserByUuidAsync(userUuid);
            if (exsistingFromUsersApi == null)
            {
                return new NotFoundObjectResult(new { Message = "The User is not found" });
            }
            
            return new AcceptedResult();
        }
    }
}
