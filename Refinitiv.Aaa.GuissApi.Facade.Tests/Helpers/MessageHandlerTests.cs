using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Refinitiv.Aaa.Interfaces.Messaging;
using Refinitiv.Aaa.MessageBus.Amazon.Models;
using Refinitiv.Aaa.GuissApi.Facade.Helpers;
using Refinitiv.Aaa.GuissApi.Facade.Interfaces;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.UserAttribute;
using Action = Refinitiv.Aaa.Interfaces.Business.Action;

namespace Refinitiv.Aaa.GuissApi.Facade.Tests.Helpers
{
    [TestFixture]
    public class MessageHandlerTests
    {
        private class Payload
        {
            public string Id { get; set; }
        }

        private MessageHandler fixture;
        private Mock<IAppSettingsConfiguration> appSettings;
        private Mock<IMessageQueue> messageQueue;
        private Mock<IMessagePublisher> messagePublisher;

        [SetUp]
        public void Setup()
        {
            appSettings = new Mock<IAppSettingsConfiguration>();
            appSettings.SetupGet(s => s.ApplicationId).Returns("APP_ID");

            messageQueue = new Mock<IMessageQueue>();
            messageQueue.Setup(m => m.SendMessageAsync(It.IsAny<string>(), It.IsAny<IMessage<Payload>>(), default))
                .Returns(() => Task.CompletedTask);

            messagePublisher = new Mock<IMessagePublisher>();
            messagePublisher.Setup(m => m.PublishMessageAsync(It.IsAny<string>(), It.IsAny<IMessage<Payload>>(), default))
                .Returns(() => Task.CompletedTask);

            fixture = new MessageHandler(
                appSettings.Object,
                messageQueue.Object,
                messagePublisher.Object
            );
        }

        [Test]
        public void SendMessage_WhenMessageIsNull_ThrowsException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => fixture.SendMessageAsync<UserAttribute>("Guiss", null));
        }

        [Test]
        public async Task SendMessage_SendsAMessageToTheSQSAsync()
        {
            var message = new Message<Payload>("Guiss", Action.Create, "PAYLOAD_ID")
            {
                Content = new Payload
                {
                    Id = "PAYLOAD_ID"
                }
            };
            await fixture.SendMessageAsync("Guiss", message, default);

            messageQueue.Verify(queue => queue.SendMessageAsync(
                "APP_ID",
                It.Is<IMessage<Payload>>(m => m.Content.Id == "PAYLOAD_ID"),
                default));
        }

        [Test]
        public async Task SendMessage_SendsAMessageToTheSNSAsync()
        {
            var message = new Message<Payload>("Guiss", Action.Create, "PAYLOAD_ID")
            {
                Content = new Payload
                {
                    Id = "PAYLOAD_ID"
                }
            };
            await fixture.SendMessageAsync("Guiss", message, default);

            messagePublisher.Verify(queue => queue.PublishMessageAsync(
                "Guiss",
                It.Is<IMessage<Payload>>(m => m.Content.Id == "PAYLOAD_ID"),
                default));
        }
    }
}
