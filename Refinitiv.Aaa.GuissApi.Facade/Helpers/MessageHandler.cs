using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Refinitiv.Aaa.Interfaces.Messaging;
using Refinitiv.Aaa.GuissApi.Facade.Interfaces;

[assembly: InternalsVisibleTo("Refinitiv.Aaa.GuissApi.Facade.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace Refinitiv.Aaa.GuissApi.Facade.Helpers
{
    /// <summary>
    /// Handles publishing messages to both SNS and SQS.
    /// </summary>
    internal sealed class MessageHandler : IMessageHandler
    {
        private readonly IAppSettingsConfiguration appSettings;
        private readonly IMessageQueue messageQueue;
        private readonly IMessagePublisher messagePublisher;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandler"/> class.
        /// </summary>
        /// <param name="appSettings">Application settings.</param>
        /// <param name="messageQueue">SQS Message queue.</param>
        /// <param name="messagePublisher">SNS Message publisher.</param>
        public MessageHandler(
            IAppSettingsConfiguration appSettings,
            IMessageQueue messageQueue,
            IMessagePublisher messagePublisher)
        {
            this.appSettings = appSettings;
            this.messageQueue = messageQueue;
            this.messagePublisher = messagePublisher;
        }

        /// <summary>
        /// Send a message to all buses.
        /// </summary>
        /// <param name="topicType">The target topic.</param>
        /// <param name="message">The actual message to send.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <typeparam name="T">Payload Type.</typeparam>
        /// <returns>A task that completes when both messages have been sent.</returns>
        public Task SendMessageAsync<T>(string topicType, IMessage<T> message, CancellationToken cancellationToken = default)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            return SendMessageInternalAsync(topicType, message, cancellationToken);
        }

        private async Task SendMessageInternalAsync<T>(
            string topicType,
            IMessage<T> message,
            CancellationToken cancellationToken = default)
        {
            await Task.WhenAll(
                    messageQueue.SendMessageAsync(appSettings.ApplicationId, message, cancellationToken),
                    messagePublisher.PublishMessageAsync(topicType, message, cancellationToken))
                .ConfigureAwait(false);
        }
    }
}
