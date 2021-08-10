using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Refinitiv.Aaa.Foundation.ApiClient.Core.Models.User;
using Refinitiv.Aaa.Foundation.ApiClient.Interfaces;
using Refinitiv.Aaa.GuissApi.Data.Interfaces;
using Refinitiv.Aaa.GuissApi.Data.Models;
using Refinitiv.Aaa.GuissApi.Facade.Interfaces;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.UserAttribute;
using System;
using System.Linq;
using System.Net.Http;
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
            return InternalValidateCommaSeparatedString(attributes);
        }

        /// <inheritdoc/>
        public IActionResult ValidateNamespacesString(string namespaces)
        {
            var validationResult = InternalValidateCommaSeparatedString(namespaces);

            if (!(validationResult is AcceptedResult))
            {
                return validationResult;
            }
            if (namespaces.Contains('.'))
            {
                return new BadRequestObjectResult(new { Message = "The namespaces can't contain dots" });
            }

            return validationResult;
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
            UserResponse exsistingFromUsersApi;
            try
            {
                exsistingFromUsersApi = await userHelper.GetUserByUuidAsync(userUuid);
            }
            catch (HttpRequestException ex)
            {
                return new BadRequestObjectResult(new { Message = "Failed to request users-api, service unavailable" });
            }

            if (exsistingFromUsersApi == null)
            {
                return new NotFoundObjectResult(new { Message = "The User is not found" });
            }
            
            return new AcceptedResult();
        }

        private IActionResult InternalValidateCommaSeparatedString(string str)
        {
            if (String.IsNullOrEmpty(str) || !str.Split(',').Any())
            {
                return new BadRequestObjectResult(new { Message = "The string is null or empty" });
            }
            return new AcceptedResult();
        }
    }
}
