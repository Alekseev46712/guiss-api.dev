using System;

namespace Refinitiv.Aaa.GuissApi.Facade.Extensions
{
    /// <summary>
    /// Provides helpers to convert strings to Enums.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Converts a string to an Enum.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="defaultValue">The value to return if conversion failed.</param>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <returns>The converted enum.</returns>
        public static TEnum ToEnum<TEnum>(this string value, TEnum defaultValue = default)
            where TEnum : struct
        {
            return Enum.TryParse<TEnum>(value, true, out var target) ? target : defaultValue;
        }
    }
}
