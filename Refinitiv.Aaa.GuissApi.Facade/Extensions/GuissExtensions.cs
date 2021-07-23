using Refinitiv.Aaa.GuissApi.Data.Models;
using Refinitiv.Aaa.GuissApi.Facade.Models;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.UserAttribute;

namespace Refinitiv.Aaa.GuissApi.Facade.Extensions
{
    /// <summary>
    /// Extension methods for Guiss and IGuissDto.
    /// </summary>
    public static class GuissExtensions
    {
        /// <summary>
        /// Maps a Guiss DTO object into a Guiss Model.
        /// </summary>
        /// <param name="userAttributeDb">Dto to map.</param>
        /// <returns>A Guiss object mapped from GuissDb.</returns>
        public static UserAttribute Map(this UserAttributeDb userAttributeDb)
        {
            if (userAttributeDb == null)
            {
                return null;
            }

            return new UserAttribute
            {
                Name = userAttributeDb.Name,
                SearchName = userAttributeDb.SearchName,
                UpdatedBy = userAttributeDb.UpdatedBy,
                UpdatedOn = userAttributeDb.UpdatedOn,
                UserUuid = userAttributeDb.UserUuid,
                Value = userAttributeDb.Value,
                Version = userAttributeDb.Version
            };
        }

        /// <summary>
        /// Maps a Guiss model into a DTO object.
        /// </summary>
        /// <param name="userAttribute">Guiss model to map.</param>
        /// <returns>A GuissDb object mapped from a Guiss.</returns>
        public static UserAttributeDb Map(this UserAttribute userAttribute)
        {
            if (userAttribute == null)
            {
                return null;
            }

            return new UserAttributeDb
            {
                Name = userAttribute.Name,
                SearchName = userAttribute.SearchName,
                UpdatedBy = userAttribute.UpdatedBy,
                UpdatedOn = userAttribute.UpdatedOn,
                UserUuid = userAttribute.UserUuid,
                Value = userAttribute.Value,
                Version = userAttribute.Version
            };
        }
    }
}
