using Refinitiv.Aaa.GuissApi.Data.Models;
using Refinitiv.Aaa.GuissApi.Facade.Models;

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
        /// <param name="db">Dto to map.</param>
        /// <returns>A Guiss object mapped from GuissDb.</returns>
        public static Guiss Map(this GuissDb db)
        {
            if (db == null)
            {
                return null;
            }

            return new Guiss
            {
                Id = db.Id,
                Version = db.Version,
                UpdatedOn = db.UpdatedOn,
                Name = db.Name,
                UpdatedBy = db.UpdatedBy,
            };
        }

        /// <summary>
        /// Maps a Guiss model into a DTO object.
        /// </summary>
        /// <param name="template">Guiss model to map.</param>
        /// <returns>A GuissDb object mapped from a Guiss.</returns>
        public static GuissDb Map(this Guiss template)
        {
            if (template == null)
            {
                return null;
            }

            return new GuissDb
            {
                Id = template.Id,
                Version = template.Version,
                UpdatedOn = template.UpdatedOn,
                Name = template.Name,
                UpdatedBy = template.UpdatedBy,
            };
        }
    }
}
