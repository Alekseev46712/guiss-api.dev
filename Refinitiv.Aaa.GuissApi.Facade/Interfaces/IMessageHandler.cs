using System.Threading;
using System.Threading.Tasks;
using Refinitiv.Aaa.Interfaces.Messaging;

namespace Refinitiv.Aaa.GuissApi.Facade.Interfaces
{
    /// <summary>
    /// Represents utility for sending a message to multiple message buses.
    /// </summary>
    internal interface IMessageHandler
    {
        /// <summary>
        /// Send a message to all buses.
        /// </summary>
        /// <param name="topicType">The target topic.</param>
        /// <param name="message">The actual message to send.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <typeparam name="T">Payload Type.</typeparam>
        /// <returns>A task that completes when all messages have been sent.</returns>
        Task SendMessageAsync<T>(string topicType, IMessage<T> message, CancellationToken cancellationToken = default);
    }
}
