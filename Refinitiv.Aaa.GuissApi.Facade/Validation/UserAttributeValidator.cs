using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Refinitiv.Aaa.Foundation.ApiClient.Interfaces;
using Refinitiv.Aaa.GuissApi.Data.Interfaces;
using Refinitiv.Aaa.GuissApi.Data.Models;
using Refinitiv.Aaa.GuissApi.Facade.Extensions;
using Refinitiv.Aaa.GuissApi.Facade.Interfaces;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.UserAttribute;
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

        /// <param name="userHelper">User Helper.</param>
        /// <param name="userAttributeHelper">User Attribute Helper</param>
        /// <param name="userAttributeRepository">User Attribute Repository.</param>
        /// <param name="mapper">Automapper.</param>
        public UserAttributeValidator(IUserHelper userHelper, IUserAttributeHelper userAttributeHelper, IUserAttributeRepository userAttributeRepository, IMapper mapper)
        {
            this.userHelper = userHelper;
            this.userAttributeRepository = userAttributeRepository;
            this.mapper = mapper;
        }

        /// <summary>
        /// Checks if User Uuid is valid by calling users api
        /// </summary>
        /// <param name="userAttribute">User Attribute Details.</param>
        /// <returns>IActionResult.</returns>
        public Task<IActionResult> ValidateAttributeAsync(UserAttribute userAttribute)
        {
            if (userAttribute == null)
            {
                throw new ArgumentNullException(nameof(userAttribute));
            }

            return InternalValidateAttributeAsync(userAttribute);
        }

        private async Task<IActionResult> InternalValidateAttributeAsync(UserAttribute userAttribute)
        {
            var exsistingFromUsersApi = await userHelper.GetUserByUuidAsync(userAttribute.UserUuid);
            if (exsistingFromUsersApi == null)
            {
                return new NotFoundObjectResult(new { Message = "The User is not found" });
            }

            return new AcceptedResult();
        }

        /// <summary>
        /// Checks if it's update request or post
        /// </summary>
        /// <param name="userAttribute">User Attribute Details.</param>
        /// <returns>UserAttribute or null.</returns>
        public Task<UserAttribute> ValidatePutRequestAsync(UserAttribute userAttribute)
        {
            if (userAttribute == null)
            {
                throw new ArgumentNullException(nameof(userAttribute));
            }

            return InternalValidatePutRequestAsync(userAttribute);
        }

        private async Task<UserAttribute> InternalValidatePutRequestAsync(UserAttribute userAttribute)
        {
            var exsistingUserAttribute = await userAttributeRepository.FindByUserUuidAndNameAsync(userAttribute.UserUuid, userAttribute.Name);

            if (exsistingUserAttribute == null)
            {
                return null;
            }

            exsistingUserAttribute.Value = userAttribute.Value;
            exsistingUserAttribute.UpdatedBy = userAttribute.UpdatedBy;
            exsistingUserAttribute.UpdatedOn = userAttribute.UpdatedOn;

            return mapper.Map<UserAttributeDb, UserAttribute>(exsistingUserAttribute);
        }

    }
}
