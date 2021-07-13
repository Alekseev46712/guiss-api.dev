using System;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using Refinitiv.Aaa.GuissApi.Facade.Exceptions;

namespace Refinitiv.Aaa.GuissApi.Facade.Tests.Exceptions
{
    [TestFixture]
    public class InvalidPaginationTokenExceptionTests
    {
        [Test]
        public void ConstructorConstructsObject()
        {
            new InvalidPaginationTokenException()
                .Should().NotBeNull();
        }

        [Test]
        public void ConstructorSetsMessage()
        {
            const string message = "error message";

            new InvalidPaginationTokenException(message)
                .Message
                .Should().Be(message);
        }

        [Test]
        public void ConstructorSetsMessageAndInnerException()
        {
            const string message = "error message";
            var innerException = new FormatException();

            var invalidToken = new InvalidPaginationTokenException(message, innerException);

            using (new AssertionScope())
            {
                invalidToken.Message.Should().Be(message);
                invalidToken.InnerException.Should().BeSameAs(innerException);
            }
        }

        [Test]
        public void ExceptionIsSerializable()
        {
            new InvalidPaginationTokenException("message", new FormatException())
                .Should().BeBinarySerializable();
        }
    }
}
