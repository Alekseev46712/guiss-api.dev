using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Refinitiv.Aaa.GuissApi.Data.Tests")]

namespace Refinitiv.Aaa.GuissApi.Data.Converters
{
    /// <summary>
    /// Converts DateTimeOffset to and from a format that can be stored in DynamoDB.
    /// </summary>
    internal class DateTimeOffsetConverter : IPropertyConverter
    {
        /// <inheritdoc cref="IPropertyConverter" />
        public DynamoDBEntry ToEntry(object value)
        {
            try
            {
                var date = (DateTimeOffset)value;
                return new Primitive(date.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture), true);
            }
            catch (InvalidCastException e)
            {
                throw new ArgumentException("Value is not of type DateTimeOffset.", nameof(value), e);
            }
        }

        /// <inheritdoc cref="IPropertyConverter" />
        public object FromEntry(DynamoDBEntry entry)
        {
            return DateTimeOffset.FromUnixTimeSeconds(entry.AsLong())
                .ToOffset(TimeSpan.Zero);
        }
    }
}
