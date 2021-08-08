using System;
using System.Runtime.Serialization;

namespace Refinitiv.Aaa.GuissApi.Facade.Exceptions
{
    /// <summary>
    /// Exception indicating that attribute is invalid or relates to the wrong provider.
    /// </summary>
    [Serializable]
    public class InvalidAttributeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidAttributeException"/> class.
        /// </summary>
        /// <param name="attributeName">Attribute name.</param>
        /// <param name="providerName">Provider name.</param>
        public InvalidAttributeException(string attributeName, string providerName)
            : base($"The attribute name '{attributeName}' is invalid " +
                   $"or relates to the wrong provider '{providerName}'.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidAttributeException"/> class.
        /// </summary>
        /// <param name="serializationInfo">Serialization information.</param>
        /// <param name="streamingContext">Streaming context.</param>
        protected InvalidAttributeException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
