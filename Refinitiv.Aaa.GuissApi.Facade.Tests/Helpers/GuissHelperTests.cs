using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Refinitiv.Aaa.Interfaces.Business;
using Refinitiv.Aaa.Interfaces.Messaging;
using Refinitiv.Aaa.Pagination.Models;
using Refinitiv.Aaa.GuissApi.Data.Exceptions;
using Refinitiv.Aaa.GuissApi.Data.Interfaces;
using Refinitiv.Aaa.GuissApi.Data.Models;
using Refinitiv.Aaa.GuissApi.Facade.Helpers;
using Refinitiv.Aaa.GuissApi.Facade.Interfaces;
using Refinitiv.Aaa.GuissApi.Facade.Models;
using Refinitiv.Aaa.Interfaces.Headers;

namespace Refinitiv.Aaa.GuissApi.Facade.Tests.Helpers
{
    [TestFixture]
    public class GuissHelperTests
    {
        private const string Uuid = "uuid";
        private IMessage sentMessage = null;
        private readonly List<GuissDb> templates = new List<GuissDb>
        {
            new GuissDb {Id = "1", Name = "Guiss 1"},
            new GuissDb {Id = "2", Name = "Guiss 2"}
        };

        private Mock<IGuissRepository> templateRepository;
        private GuissHelper fixture;
        private Mock<IMessageHandler> messageHandler;
        private Mock<IMessagePublisher> messagePublisher;
        private Mock<IAppSettingsConfiguration> appSettings;
        private Mock<IPaginationHelper> paginationHelper;
        private Mock<ILogger<GuissHelper>> logger;
        private Mock<IAaaRequestHeaders> aaaRequestHeaders;

        [SetUp]
        public void Init()
        {
            InitGuissRepository();
            InitMessageHandler();
            InitMessagePublisher();
            InitAppSettings();
            InitLogger();
            InitPaging();
            aaaRequestHeaders = new Mock<IAaaRequestHeaders>();
            InitFixture();
        }

        private void InitGuissRepository()
        {
            templateRepository = new Mock<IGuissRepository>();
            templateRepository.Setup(r => r.FindAllAsync()).ReturnsAsync(templates);

            templateRepository.Setup(r => r.FindByIdAsync(It.IsAny<string>()))
                .Returns<string>(id =>
                {
                    var template = templates.FirstOrDefault(g => g.Id == id);
                    return Task.FromResult(template);
                });

            templateRepository.Setup(x => x.GetAllAsync(It.IsAny<Cursor<GuissFilter>>()))
                .ReturnsAsync(templates);

            templateRepository.Setup(r => r.SaveAsync(It.IsAny<GuissDb>()))
                .ReturnsAsync((GuissDb dto) =>
                    new GuissDb
                    {
                        Id = dto.Id,
                        Name = dto.Name,
                        UpdatedOn = dto.UpdatedOn,
                        Version = dto.Version.GetValueOrDefault(-1) + 1
                    });
        }

        private void InitPaging()
        {
            paginationHelper = new Mock<IPaginationHelper>();
            paginationHelper.Setup(x => x.SetupCursor(It.IsAny<GuissFilter>(), It.IsAny<IResultSet<Guiss>>(), It.IsAny<IPagingModel>()))
                .Returns(() => new Cursor<GuissFilter>(1, null));
        }

        private void InitMessageHandler()
        {
            messageHandler = new Mock<IMessageHandler>();
            //messageHandler.Setup(h => h.SendMessageAsync(It.IsAny<TopicType>(), It.IsAny<IMessage<Guiss>>(), default))
             //   .Callback((TopicType topic, IMessage message, CancellationToken cancellation) => sentMessage = message)
              //  .Returns(Task.CompletedTask);
        }

        private void InitMessagePublisher()
        {
            messagePublisher = new Mock<IMessagePublisher>();
            messagePublisher.Setup(h => h.PublishMessageAsync(It.IsAny<string>(), It.IsAny<IMessage<Guiss>>(), It.IsAny<CancellationToken>()))
                .Callback((string topic, IMessage message, CancellationToken cancellation) => sentMessage = message)
                .Returns(Task.CompletedTask);
        }

        private void InitLogger()
        {
            logger = new Mock<ILogger<GuissHelper>>();
        }

        private void InitAppSettings()
        {
            appSettings = new Mock<IAppSettingsConfiguration>();
            appSettings.SetupGet(s => s.ApplicationId).Returns("APPLICATION_ID");
        }

        private void InitFixture()
        {
            fixture = new GuissHelper(
                templateRepository.Object,
                messagePublisher.Object,
                paginationHelper.Object,
                logger.Object,
                aaaRequestHeaders.Object
            );
        }

        [Test]
        public async Task FindAll_WhenFilterIsNull_ReturnsAllGuiss()
        {
            var result = await fixture.FindAllAsync();

            result.Items.Should().HaveCount(2);
            templateRepository.Verify(x => x.GetAllAsync(It.IsAny<Cursor<GuissFilter>>()), Times.Once);
        }

        [Test]
        public async Task FindAll_WhenFilterIsSet_ReturnsGuiss()
        {
            var result = await fixture.FindAllAsync(new GuissFilter
            {
                Kind = "TEMPLATE"
            });

            result.Items.Should().HaveCount(2);

            templateRepository.Verify(x => x.GetAllAsync(It.IsAny<Cursor<GuissFilter>>()), Times.Once);
        }

        [Test]
        public async Task FindAll_Sets_Previous_PaginationTokens_WithBackwardSearch()
        {
            paginationHelper.Setup(x => x.SetupCursor(It.IsAny<GuissFilter>(), It.IsAny<IResultSet<Guiss>>(), It.IsAny<IPagingModel>()))
                .Returns(() => new Cursor<GuissFilter>(2, null, "lastEvaluatedKey", true));

            paginationHelper.Setup(x => x.CreatePaginationToken(It.IsAny<Cursor<GuissFilter>>()))
                .Returns("paginationToken");

            var result = await fixture.FindAllAsync();

            result.Previous.Should().NotBeNullOrEmpty();
        }

        [Test]
        public async Task FindAll_Sets_Next_PaginationTokens_WithBackwardSearch()
        {
            paginationHelper.Setup(x => x.SetupCursor(It.IsAny<GuissFilter>(), It.IsAny<IResultSet<Guiss>>(), It.IsAny<IPagingModel>()))
                .Returns(() => new Cursor<GuissFilter>(2, null, "lastEvaluatedKey"));

            paginationHelper.Setup(x => x.CreatePaginationToken(It.IsAny<Cursor<GuissFilter>>()))
                .Returns("paginationToken");

            var result = await fixture.FindAllAsync();

            result.Next.Should().NotBeNullOrEmpty();
        }

        [Test]
        public async Task FindById_IfGuissDoesNotExist_ReturnsNull()
        {
            var result = await fixture.FindByIdAsync("INVALID_ID");
            Assert.IsNull(result);
        }

        [Test]
        public async Task FindById_IfGuissExists_ReturnsGuiss()
        {
            var result = await fixture.FindByIdAsync("1");
            Assert.IsInstanceOf<Guiss>(result);
        }

        [Test]
        public void Insert_IfGuissHasNotBeenSpecified_ThrowArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => fixture.InsertAsync(null));
        }

        [Test]
        public async Task Insert_CallsSaveMethodOnRepository()
        {
            var template = new Guiss
            {
                Name = "GuissName",
                UpdatedOn = new DateTimeOffset(2019, 6, 21, 10, 19, 0, TimeSpan.Zero)
            };

            await fixture.InsertAsync(@template);

            templateRepository.Verify(r => r.SaveAsync(
                It.Is<GuissDb>(dto =>
                   dto.Name == template.Name
                   && dto.UpdatedOn == template.UpdatedOn)));
        }

        [Test]
        public async Task Insert_CreatesAValidMessageToPublish()
        {
            var template = new Guiss
            {
                Name = "New Guiss",
                UpdatedOn = DateTimeOffset.Now
            };

            await fixture.InsertAsync(@template);

            Func<IMessage, bool> validateContent = message =>
            {
                var content = message.Content as Guiss;

                return !string.IsNullOrEmpty(content?.Id) && content.Name == template.Name;
            };

            messagePublisher.Verify(
                p => p.PublishMessageAsync(
                    It.IsAny<string>(),
                    It.Is<IMessage<Guiss>>(c => validateContent(c)),
                    default
                ),
                Times.Once
            );
        }

        [Test]
        public async Task Insert_SetsTheVersionOfMessageToTheSameAsTheGuiss()
        {
            var template = new Guiss
            {
                Name = Guid.NewGuid().ToString(),
                UpdatedOn = DateTimeOffset.Now
            };

            messageHandler.Setup(
                p => p.SendMessageAsync(
                    It.IsAny<string>(),
                    It.IsAny<IMessage<Guiss>>(),
                    default
                )
            )
            .Returns(Task.CompletedTask);

            await fixture.InsertAsync(@template);

            messagePublisher.Verify(
                p => p.PublishMessageAsync(
                    It.IsAny<string>(),
                    It.Is<IMessage<Guiss>>(c => c.Content.Version == 0),
                    It.IsAny<CancellationToken>()
                )
            );
        }

        [Test]
        public async Task Insert_SetsTheUpdatedOnOfMessageToTheSameAsTheGuiss()
        {
            var template = new Guiss
            {
                Name = Guid.NewGuid().ToString(),
                UpdatedOn = DateTimeOffset.Now
            };

            messageHandler.Setup(
                p => p.SendMessageAsync(
                    It.IsAny<string>(),
                    It.IsAny<IMessage<Guiss>>(),
                    default
                )
            )
            .Returns(Task.CompletedTask);

            await fixture.InsertAsync(@template);

            messagePublisher.Verify(
                p => p.PublishMessageAsync(
                    It.IsAny<string>(),
                    It.Is<IMessage<Guiss>>(c => c.Content.UpdatedOn == template.UpdatedOn),
                    It.IsAny<CancellationToken>()
                )
            );
        }

        [Test]
        public async Task Insert_ReturnsTheNewId()
        {
            var template = new Guiss { Name = "New Guiss" };
            var result = await fixture.InsertAsync(@template);

            Assert.IsNotEmpty(result.Id);
        }

        [Test]
        public void Update_IfGuissHasNotBeenSpecified_ThrowArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => fixture.UpdateAsync(null));
        }

        [Test]
        public async Task Update_CallsSaveMethodOnRepository()
        {
            var template = new Guiss
            {
                Id = "THE_ID",
                Name = "New Guiss",
                Version = 1,
                UpdatedOn = DateTimeOffset.Now
            };

            await fixture.UpdateAsync(@template);

            templateRepository.Verify(
                r => r.SaveAsync(
                    It.Is<GuissDb>(dto =>
                        dto.Id == template.Id &&
                        dto.Name == template.Name &&
                        dto.UpdatedOn == template.UpdatedOn &&
                        dto.Version == template.Version
                    )
                ),
                Times.Once
            );
        }

        [Test]
        public void Update_AllowsUpdateConflictExceptionToBubbleUp()
        {
            templateRepository.Setup(r => r.SaveAsync(It.IsAny<GuissDb>()))
                .ThrowsAsync(new UpdateConflictException());

            var template = new Guiss
            {
                Name = Guid.NewGuid().ToString()
            };

            Assert.ThrowsAsync<UpdateConflictException>(() => fixture.UpdateAsync(template));
        }

        [Test]
        public async Task Update_PublishesAGuissUpdatedNotification()
        {
            var template = new Guiss
            {
                Id = "THE_ID",
                Name = "New Guiss",
                Version = 1,
                UpdatedOn = DateTimeOffset.Now
            };

            await fixture.UpdateAsync(template);

            Func<IMessage, bool> validate = (message) =>
            {
                var content = message.Content as Guiss;
                return content.Id == template.Id &&
                       content.Name == template.Name &&
                       content.UpdatedOn == template.UpdatedOn &&
                       content.Version == template.Version + 1;
            };

            messagePublisher.Verify(p =>
                p.PublishMessageAsync(
                    It.IsAny<string>(),
                    It.Is<IMessage<Guiss>>(c => validate(c)),
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );
        }

        [Test]
        public async Task Update_ReturnsTheNewId()
        {
            var template = new Guiss { Id = "1", Name = "New Guiss" };
            var result = await fixture.UpdateAsync(@template);

            Assert.IsNotEmpty(result.Id);
        }

        [Test]
        public async Task Delete_CallsDeleteMethodOnRepository()
        {
            const string id = "1234";
            await fixture.DeleteAsync(id);
            templateRepository.Verify(r => r.DeleteAsync(id), Times.Once);
        }

        [Test]
        public async Task Delete_PublishesAGuissDeletedNotification()
        {
            await fixture.DeleteAsync("1");

            Func<IMessage, bool> validate = (message) =>
            {
                var content = message.Content as Guiss;
                return content.Id == "1";
            };

            messagePublisher.Verify(
                p => p.PublishMessageAsync(
                    It.IsAny<string>(),
                    It.Is<IMessage<Guiss>>(c => validate(c)),
                    default
                ),
                Times.Once
            );
        }

        [Test]
        public async Task FindAll_WhenPageLimitSet()
        {
            templateRepository.Setup(x => x.GetAllAsync(It.IsAny<Cursor<GuissFilter>>()))
                .ReturnsAsync(() => new List<GuissDb>
                {
                    templates.First()
                });

            var result = await fixture.FindAllAsync(null, new Facade.Models.PaginationModel { Limit = 1 });
            Assert.AreEqual(1, result.Items.Count());
        }
    }
}
