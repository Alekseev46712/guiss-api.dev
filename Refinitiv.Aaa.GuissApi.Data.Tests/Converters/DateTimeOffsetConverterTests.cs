using Amazon.DynamoDBv2.DocumentModel;
using FluentAssertions;
using NUnit.Framework;
using Refinitiv.Aaa.GuissApi.Data.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Refinitiv.Aaa.GuissApi.Data.Tests.Converters
{
    [TestFixture]
    public class DateTimeOffsetConverterTests
    {
        private DateTimeOffsetConverter _converter;
        private DateTimeOffset _testDate;
        private long _testDateUnixSeconds;

        [SetUp]
        public void SetUp()
        {
            _converter = new DateTimeOffsetConverter();
            _testDate = new DateTimeOffset(2019, 8, 12, 17, 0, 0, TimeSpan.Zero);
            _testDateUnixSeconds = _testDate.ToUnixTimeSeconds();
        }

        [Test]
        public void ToEntryConvertsDateTimeOffsetToUnixSeconds()
        {
            var dynamoEntry = _converter.ToEntry(_testDate);

            dynamoEntry.AsLong()
                .Should().Be(_testDateUnixSeconds, "because the date should be stored as seconds since the Unix epoch");
        }

        [Test]
        public void ToEntryThrowsArgumentExceptionIfValueIsNotDateTimeOffset()
        {
            _converter.Invoking(c => c.ToEntry("not a date"))
                .Should().Throw<ArgumentException>("because the value is not a DateTimeOffset")
                .WithMessage("Value is not of type DateTimeOffset.*")
                .Which.ParamName.Should().Be("value");
        }

        [Test]
        public void FromEntryConvertsUnixSecondsToDateTimeOffset()
        {
            var dynamoEntry = new Primitive(_testDateUnixSeconds.ToString(), true);

            _converter.FromEntry(dynamoEntry)
                .Should().BeOfType<DateTimeOffset>()
                .Which.Should().Be(_testDate);
        }
    }

}
