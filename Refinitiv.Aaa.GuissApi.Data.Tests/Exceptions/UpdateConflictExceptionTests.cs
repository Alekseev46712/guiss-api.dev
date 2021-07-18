using System;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using Refinitiv.Aaa.GuissApi.Data.Exceptions;

namespace Refinitiv.Aaa.GuissApi.Data.Tests.Exceptions
{
    /// <summary>
    /// Unit tests for <see cref="UpdateConflictException"/>.
    /// </summary>
    [TestFixture]
    public class UpdateConflictExceptionTests
    {
        private const string DefaultErrorMessage = "The item has been updated by another user.";

        [Test]
        public void DefaultConstructorSetsDefaultErrorMessage()
        {
            new UpdateConflictException()
                .Message
                .Should().Be(DefaultErrorMessage);
        }

        [Test]
        public void ConstructorSetsErrorMessage()
        {
            var message = "An error has occurred.";

            new UpdateConflictException(message)
                .Message
                .Should().Be(message);
        }

        [Test]
        public void ConstructorSetsErrorMessageAndInnerException()
        {
            var message = "An error has occurred.";
            var innerException = new InvalidOperationException("test");
            var exception = new UpdateConflictException(message, innerException);

            using (new AssertionScope())
            {
                exception.Message.Should().Be(message);
                exception.InnerException.Should().BeSameAs(innerException);
            }
        }

        [Test]
        public void ConstructorSetsInnerExceptionAndDefaultErrorMessage()
        {
            var innerException = new InvalidOperationException("test");
            var exception = new UpdateConflictException(innerException);

            using (new AssertionScope())
            {
                exception.Message.Should().Be(DefaultErrorMessage);
                exception.InnerException.Should().BeSameAs(innerException);
            }
        }
    }
}
