using System;
using Refinitiv.Aaa.Interfaces.Business;
using Refinitiv.Aaa.Pagination.Interfaces;
using Refinitiv.Aaa.Pagination.Models;
using Refinitiv.Aaa.GuissApi.Data.Models;
using Refinitiv.Aaa.GuissApi.Facade.Exceptions;
using Refinitiv.Aaa.GuissApi.Facade.Interfaces;

namespace Refinitiv.Aaa.GuissApi.Facade.Helpers
{
    /// <inheritdoc />
    public class PaginationHelper : IPaginationHelper
    {
        private readonly IAppSettingsConfiguration appSettingsConfiguration;
        private readonly IPaginationService paginationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaginationHelper"/> class.
        /// </summary>
        /// <param name="appSettingsConfiguration">Instance of IAppSettingsConfiguration.</param>
        /// <param name="paginationService">Instance of IPaginationService.</param>
        public PaginationHelper(
            IAppSettingsConfiguration appSettingsConfiguration,
            IPaginationService paginationService)
        {
            this.appSettingsConfiguration = appSettingsConfiguration;
            this.paginationService = paginationService;
        }

        /// <inheritdoc />
        public Cursor<GuissFilter> SetupCursor<TResultSet>(GuissFilter filterValue, IResultSet<TResultSet> resultSet, IPagingModel paging = null)
            where TResultSet : IModel
        {
            if (resultSet == null)
            {
                throw new ArgumentNullException(nameof(resultSet));
            }

            var pageLimit = appSettingsConfiguration.DefaultQueryLimit;

            if (paging != null && paging.Limit != 0)
            {
                pageLimit = paging.Limit;
            }

            var cursor = new Cursor<GuissFilter>(pageLimit, filterValue);

            if (string.IsNullOrEmpty(paging?.Pagination))
            {
                resultSet.Previous = CreatePaginationToken(cursor);
                return cursor;
            }

            try
            {
                cursor = paginationService.GetPageObject<GuissFilter>(paging.Pagination);
            }
            catch (FormatException ex)
            {
                throw new InvalidPaginationTokenException("The pagination token is invalid.", ex);
            }

            if (string.IsNullOrEmpty(cursor.LastEvaluatedKey) || cursor.LastEvaluatedKey.Equals("{}", StringComparison.CurrentCultureIgnoreCase))
            {
                return cursor;
            }

            // set the incoming pagination token for either next/previous property
            if (cursor.BackwardSearch)
            {
                resultSet.Next = CreatePaginationToken(new Cursor<GuissFilter>(cursor.Limit, filterValue, cursor.LastEvaluatedKey));
            }
            else
            {
                resultSet.Previous = CreatePaginationToken(new Cursor<GuissFilter>(cursor.Limit, filterValue, cursor.LastEvaluatedKey, true));
            }

            return cursor;
        }

        /// <inheritdoc />
        public string CreatePaginationToken(Cursor<GuissFilter> cursor)
        {
            var paginationToken = paginationService.CreatePageToken(cursor);
            return !string.IsNullOrEmpty(paginationToken) ? paginationToken : null;
        }
    }
}
