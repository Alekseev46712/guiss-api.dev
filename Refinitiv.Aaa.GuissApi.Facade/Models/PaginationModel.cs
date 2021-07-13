using System.Diagnostics.CodeAnalysis;
using Refinitiv.Aaa.Interfaces.Business;

namespace Refinitiv.Aaa.GuissApi.Facade.Models
{
    /// <summary>
    /// Criteria for paging.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class PaginationModel : IPagingModel
    {
        /// <inheritdoc />
        public int Limit { get; set; }

        /// <inheritdoc />
        public string Pagination { get; set; }
    }
}
