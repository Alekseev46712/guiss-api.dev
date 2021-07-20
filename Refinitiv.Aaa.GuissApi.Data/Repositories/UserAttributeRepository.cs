using Refinitiv.Aaa.GuissApi.Data.Interfaces;
using Refinitiv.Aaa.GuissApi.Data.Models;
using Refinitiv.Aaa.GuissApi.Interfaces.UserAttribute;
using Refinitiv.Aaa.Pagination.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Refinitiv.Aaa.GuissApi.Data.Repositories
{
    /// <inheritdoc />
    public class UserAttributeRepository : IUserAttributeRepository
    {
        /// <inheritdoc />
        public Task<(List<UserAttributeDb>, string FirstItemToken, string LastItemToken)> SearchAsync(
            Cursor<UserAttributeFilter> cursor, List<string> attributeNames = null)
        {
            // stub, will be extended
            if (cursor == null)
            {
                throw new ArgumentNullException(nameof(cursor));
            }

            return Task.FromResult((new List<UserAttributeDb> { }, string.Empty, string.Empty));
        }

        /// <inheritdoc />
        public Task<UserAttributeDb> SaveAsync(UserAttributeDb item)
        {
            // stub, will be extended
            return Task.FromResult(item);
        }

        /// <inheritdoc/>
        public async Task DeleteAsync(string userUuid)
        {
            // stub, will be extended
            await Task.CompletedTask;
        }
    }
}
