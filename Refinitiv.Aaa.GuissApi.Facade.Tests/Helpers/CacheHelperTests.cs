using AutoFixture;
using Enyim.Caching;
using Enyim.Caching.Memcached;
using Enyim.Caching.Memcached.Results;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Refinitiv.Aaa.GuissApi.Facade.Helpers;
using Refinitiv.Aaa.GuissApi.Facade.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Refinitiv.Aaa.GuissApi.Facade.Tests.Helpers
{
    [TestFixture]
    public class CacheHelperTests
    {
        private Mock<IMemcachedResultsClient> client;
        private Mock<IOptions<CacheHelperOptions>> options;
        private Mock<IOptions<CacheHelperOptions>> diabledOptions;
        private CacheHelper cacheHelper;
        private CacheHelper disabledCacheHelper;
        private IFixture fixture;

        [SetUp]
        public void Setup()
        {
            client = new Mock<IMemcachedResultsClient>();

            options = new Mock<IOptions<CacheHelperOptions>>();
            options.Setup(x => x.Value).Returns(new CacheHelperOptions { DefaultExpirationInSeconds = 300, Enabled = true });

            diabledOptions = new Mock<IOptions<CacheHelperOptions>>();
            diabledOptions.Setup(x => x.Value).Returns(new CacheHelperOptions { DefaultExpirationInSeconds = 300, Enabled = false });

            cacheHelper = new CacheHelper(options.Object, client.Object);
            disabledCacheHelper = new CacheHelper(diabledOptions.Object, client.Object);

            fixture = new Fixture();
        }

        [Test]
        public void Remove_WhenResultSuccess_ReturnsTrue()
        {
            client.Setup(x => x.ExecuteRemove(It.IsAny<string>())).Returns(new RemoveOperationResult { Success = true });

            var expected = true;
            var result = cacheHelper.Remove("key");

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Remove_WhenResultFail_ReturnsFalse()
        {
            client.Setup(x => x.ExecuteRemove(It.IsAny<string>())).Returns(new RemoveOperationResult { Success = false });

            var expected = false;
            var result = cacheHelper.Remove("key");

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Replace_WhenResultSuccess_ReturnsTrue()
        {
            client.Setup(x => x.ExecuteStore(StoreMode.Set, It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>())).Returns(new StoreOperationResult { Success = true });

            var expected = true;
            var result = cacheHelper.Replace("key", "value");

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Replace_WhenResultFail_ReturnsFalse()
        {
            client.Setup(x => x.ExecuteStore(StoreMode.Set, It.IsAny<string>(),It.IsAny<object>(),It.IsAny<TimeSpan>())).Returns(new StoreOperationResult { Success = false });

            var expected = false;
            var result = cacheHelper.Replace("key","value");

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Add_WhenResultSuccess_ReturnsTrue()
        {
            client.Setup(x => x.ExecuteStore(StoreMode.Add, It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>())).Returns(new StoreOperationResult { Success = true });

            var expected = true;
            var result = cacheHelper.Add("key", "value");

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Add_WhenResultFail_ReturnsFalse()
        {
            client.Setup(x => x.ExecuteStore(StoreMode.Add, It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>())).Returns(new StoreOperationResult { Success = false });

            var expected = false;
            var result = cacheHelper.Add("key", "value");

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void CreateOrReplace_WhenItemIsInCache_Replace()
        {
            client.Setup(x => x.ExecuteStore(StoreMode.Set, It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>())).Returns(new StoreOperationResult { Success = false });
            client.Setup(x => x.ExecuteGet(It.IsAny<string>())).Returns(new GetOperationResult { Success = true });

            cacheHelper.CreateOrReplace("key", "value");

            client.Verify(c => c.ExecuteStore(StoreMode.Set, It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>()), Times.Once());
        }

        [Test]
        public void CreateOrReplace_WhenItemIsNotInCache_Add()
        {
            client.Setup(x => x.ExecuteStore(StoreMode.Add, It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>())).Returns(new StoreOperationResult { Success = false });
            client.Setup(x => x.ExecuteGet(It.IsAny<string>())).Returns(new GetOperationResult { Success = false });

            cacheHelper.CreateOrReplace("key", "value");
            
            client.Verify(c => c.ExecuteStore(StoreMode.Add, It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>()), Times.Once());
        }

        [TestCase(true,false)]
        [TestCase(true, true)]
        [TestCase(false,true)]
        [TestCase(false, false)]
        public void CreateOrReplace_ReturnsCorrectResult(bool getOpResult, bool storeResult)
        {
            client.Setup(x => x.ExecuteGet(It.IsAny<string>())).Returns(new GetOperationResult { Success = getOpResult });
            client.Setup(x => x.ExecuteStore(It.IsAny<StoreMode>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>())).Returns(new StoreOperationResult { Success = storeResult });

            var result = cacheHelper.CreateOrReplace("key", "value");

            Assert.AreEqual(storeResult, result);
        }

        [Test]
        public void Get_ReturnsValueOnKey()
        {
            var val = "val";
            client.Setup(m => m.ExecuteGet(It.IsAny<string>())).Returns(new GetOperationResult{ Value = val });

            var result = cacheHelper.Get<string>("someKey");

            Assert.AreEqual(val, result);
        }

        [Test]
        public async Task GetValueOrCreateAsync_EnabledFalse_ReturnsFuncResult() 
        {
            Task<string> TestMethod()
            {
                return Task.FromResult<string>("value");
            }

            Func<Task<string>> act = async () => await TestMethod();

            var expected = await act.Invoke();

            var result = await disabledCacheHelper.GetValueOrCreateAsync("key",  async () => await TestMethod());

            Assert.AreEqual(result, expected);
        }

        [Test]
        public async Task GetValueOrCreateAsync_EnabledTrue_ReturnsFuncResult()
        {
            Task<string> TestMethod()
            {
                return Task.FromResult<string>("value");
            }

            client.Setup(x => x.ExecuteGet(It.IsAny<string>())).Returns(new GetOperationResult { Success = false });
            client.Setup(x => x.ExecuteStore(StoreMode.Add, It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>()))
                .Returns(new StoreOperationResult { Success = true });

            Func<Task<string>> act = async () => await TestMethod();

            var expected = await act.Invoke();

            var result = await cacheHelper.GetValueOrCreateAsync("key", async () => await TestMethod());

            client.VerifyAll();

            Assert.AreEqual(result, expected);
        }

        [Test]
        public async Task GetValueOrCreateAsync_EnabledTrueGetSuccess_ReturnsFuncResult()
        {
            var value = "someValue";

            client.Setup(x => x.ExecuteGet(It.IsAny<string>())).Returns(new GetOperationResult { Success = true, Value = value });

            var result = await cacheHelper.GetValueOrCreateAsync("key", fixture.Create<Func<Task<string>>>());

            Assert.AreEqual(value, result);
        }
    }
}
