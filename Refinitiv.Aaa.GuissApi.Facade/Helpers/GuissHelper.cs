using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Refinitiv.Aaa.Interfaces.Business;
using Refinitiv.Aaa.Interfaces.Messaging;
using Refinitiv.Aaa.MessageBus.Amazon.Models;
using Refinitiv.Aaa.Pagination.Models;
using Refinitiv.Aaa.GuissApi.Data.Interfaces;
using Refinitiv.Aaa.GuissApi.Data.Models;
using Refinitiv.Aaa.GuissApi.Facade.Interfaces;
using Refinitiv.Aaa.GuissApi.Facade.Models;
using Refinitiv.Aaa.Interfaces.Headers;
using Action = Refinitiv.Aaa.Interfaces.Business.Action;

namespace Refinitiv.Aaa.GuissApi.Facade.Helpers
{
    /// <summary>
    /// Handles all operations that need to be performed on a template.
    /// </summary>
    internal sealed class GuissHelper : IGuissHelper
    {
        private readonly IGuissRepository templateRepository;
        private readonly IMessagePublisher messagePublisher;
        private readonly IPaginationHelper paginationHelper;
        private readonly ILogger<GuissHelper> logger;
        private readonly IAaaRequestHeaders aaaRequestHeaders;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuissHelper"/> class.
        /// </summary>
        /// <param name="templateRepository">Repository used to access the data.</param>
        /// <param name="messagePublisher">Service used to add messages to a message bus.</param>
        /// <param name="paginationHelper">Instance of IPaginationHelper.</param>
        /// <param name="logger">Logging interface.</param>
        /// <param name="aaaRequestHeaders">Service for the refinitivUuid header.</param>
        public GuissHelper(
            IGuissRepository templateRepository,
            IMessagePublisher messagePublisher,
            IPaginationHelper paginationHelper,
            ILogger<GuissHelper> logger,
            IAaaRequestHeaders aaaRequestHeaders)
        {
            this.templateRepository = templateRepository;
            this.messagePublisher = messagePublisher;
            this.paginationHelper = paginationHelper;
            this.logger = logger;
            this.aaaRequestHeaders = aaaRequestHeaders;
        }

        /// <inheritdoc />
        public async Task<IResultSet<Guiss>> FindAllAsync(GuissFilter filter = null, IPagingModel paging = null)
        {
            var templateResultSet = new GuissResultSet();

            var cursor = paginationHelper.SetupCursor(filter, templateResultSet, paging);

            var templates = await templateRepository.GetAllAsync(cursor).ConfigureAwait(false);

            if (cursor.BackwardSearch)
            {
                templateResultSet.Previous = paginationHelper.CreatePaginationToken(new Cursor<GuissFilter>(cursor.Limit, filter, cursor.LastEvaluatedKey, true));
            }
            else
            {
                templateResultSet.Next = paginationHelper.CreatePaginationToken(new Cursor<GuissFilter>(cursor.Limit, filter, cursor.LastEvaluatedKey));
            }

            //templateResultSet.Items = templates.Select(o => o.Map());

            return null;//templateResultSet;
        }

        /// <inheritdoc />
        public async Task<Guiss> FindByIdAsync(string id)
        {
            var item = await templateRepository.FindByIdAsync(id).ConfigureAwait(false);
            return null;//item?.Map();
        }

        /// <inheritdoc />
        public Task<Guiss> InsertAsync(Guiss item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return InsertGuissAsync(item);
        }

        /// <inheritdoc />
        public Task<Guiss> UpdateAsync(Guiss item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return UpdateGuissAsync(item);
        }

        /// <inheritdoc />
        public async Task DeleteAsync(string id)
        {
            logger.LogInformation($"Deleting template with id {id}.");

            await templateRepository.DeleteAsync(id).ConfigureAwait(false);
            var message = GetGuissDeletedMessage(id);
            await messagePublisher.PublishMessageAsync("Guiss", message).ConfigureAwait(false);
        }

        private async Task<Guiss> InsertGuissAsync(Guiss item)
        {
            //var dto = item.Map();
            //dto.Id = Guid.NewGuid().ToString();

            //var savedGuiss = await templateRepository.SaveAsync(dto).ConfigureAwait(false);
            //var newGuiss = savedGuiss.Map();
            //var message = GetGuissCreatedMessage(newGuiss);

            //await messagePublisher.PublishMessageAsync("Guiss", message).ConfigureAwait(false);

            return null;// newGuiss;
        }

        private async Task<Guiss> UpdateGuissAsync(Guiss item)
        {
            logger.LogInformation($"Updating template with id {item.Id}.");

            //var dto = item.Map();

            //var savedGuiss = await templateRepository.SaveAsync(dto).ConfigureAwait(false);
            //var newGuiss = savedGuiss.Map();
            // var message = GetGuissUpdatedMessage(newGuiss);

            //await messagePublisher.PublishMessageAsync("Guiss", message).ConfigureAwait(false);

            return null;// newGuiss;
        }

        private IMessage<Guiss> GetGuissCreatedMessage(Guiss template)
        {
            return GetMessage(Action.Create, () => template);
        }

        private IMessage<Guiss> GetGuissUpdatedMessage(Guiss template)
        {
            return GetMessage(Action.Update, () => template);
        }

        private IMessage<Guiss> GetGuissDeletedMessage(string id)
        {
            return GetMessage(Action.Delete, () => new Guiss { Id = id });
        }

        private IMessage<Guiss> GetMessage(
            Action action,
            Func<Guiss> funcToGroup)
        {
            var template = funcToGroup();
            return new Message<Guiss>("Guiss", action, template.Id)
            {
                CorrelationId = aaaRequestHeaders.CorrelationId,
                Content = template,
                CallerUuid = aaaRequestHeaders.RefinitivUuid,
                Namespace = aaaRequestHeaders.Namespace,
            };
        }
    }
}
