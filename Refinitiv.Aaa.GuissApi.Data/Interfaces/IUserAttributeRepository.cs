using Refinitiv.Aaa.GuissApi.Data.Models;
using Refinitiv.Aaa.GuissApi.Interfaces.UserAttribute;
using Refinitiv.Aaa.Pagination.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Refinitiv.Aaa.GuissApi.Data.Interfaces
{
    public interface IUserAttributeRepository
    {
        Task<(List<UserAttributeDb>, string FirstItemToken, string LastItemToken)> SearchAsync(
            Cursor<UserAttributeFilter> cursor, List<string> attributeNames = null);

        Task<UserAttributeDb> SaveAsync(UserAttributeDb item);

        Task DeleteAsync(string userUuid);
    }
}
