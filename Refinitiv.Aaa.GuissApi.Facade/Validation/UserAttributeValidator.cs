using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Refinitiv.Aaa.Foundation.ApiClient.Interfaces;
using Refinitiv.Aaa.GuissApi.Data.Interfaces;
using Refinitiv.Aaa.GuissApi.Data.Models;
using Refinitiv.Aaa.GuissApi.Facade.Extensions;
using Refinitiv.Aaa.GuissApi.Facade.Interfaces;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.UserAttribute;
using Refinitiv.Aaa.Interfaces.Headers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Refinitiv.Aaa.GuissApi.Facade.Validation
{
    /// <summary>
    /// Validets requests
    /// </summary>
    public class UserAttributeValidator : IUserAttributeValidator
    {
        private readonly IUserHelper userHelper;
        private readonly IMapper mapper;
        private readonly IUserAttributeRepository userAttributeRepository;
        private readonly IAaaRequestHeaders aaaRequestHeaders;

        /// <param name="userHelper">User Helper.</param>
        /// <param name="userAttributeHelper">User Attribute Helper</param>
        /// <param name="userAttributeRepository">User Attribute Repository.</param>
        /// <param name="mapper">Automapper.</param>
        /// <param name="aaaRequestHeaders">Request headers.</param>
        public UserAttributeValidator(IUserHelper userHelper,
            IUserAttributeHelper userAttributeHelper,
            IUserAttributeRepository userAttributeRepository,
            IMapper mapper,
            IAaaRequestHeaders aaaRequestHeaders)
        {
            this.userHelper = userHelper;
            this.userAttributeRepository = userAttributeRepository;
            this.mapper = mapper;
            this.aaaRequestHeaders = aaaRequestHeaders;
        }

        /// <summary>
        /// Checks if User Uuid is valid by calling users api
        /// </summary>
        /// <param name="userAttributeDetails">User Attribute Details.</param>
        /// <returns>IActionResult.</returns>
        public Task<IActionResult> ValidateAttributeAsync(UserAttributeDetails userAttributeDetails)
        {
            if (userAttributeDetails == null)
            {
                throw new ArgumentNullException(nameof(userAttributeDetails));
            }

            return InternalValidateAttributeAsync(userAttributeDetails);
        }

        /// <summary>
        /// Checks if it's update request or post, if it is put returns model to update
        /// </summary>
        /// <param name="userAttributeDetails">User Attribute Details.</param>
        /// <returns>UserAttribute or null.</returns>
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
