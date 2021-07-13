using System.Net;
using NUnit.Framework;
using Refinitiv.Aaa.GuissApi.Facade.Extensions;

namespace Refinitiv.Aaa.GuissApi.Facade.Tests.Extensions
{
    [TestFixture]
    public class EnumExtensionsTests
    {
        [Test]
        public void ToEnum_WhenValueIsNull_ReturnDefaultValue()
        {
            string value = null;
            Assert.AreEqual((HttpStatusCode)0, value.ToEnum<HttpStatusCode>());
        }

        [Test]
        public void ToEnum_WhenValueIsInvalid_ReturnDefaultValue()
        {
            Assert.AreEqual((HttpStatusCode)0, "INVALID".ToEnum<HttpStatusCode>());
        }

        [Test]
        public void ToEnum_WhenValueIsInvalidAndDefaultSpecified_ReturnSpecifiedDefault()
        {
            Assert.AreEqual(HttpStatusCode.OK, "INVALID".ToEnum(HttpStatusCode.OK));
        }

        [Test]
        public void ToEnum_WhenValueIsValid_ReturnConvertedEnum()
        {
            Assert.AreEqual(HttpStatusCode.OK, "OK".ToEnum(HttpStatusCode.Created));
        }
    }
}
