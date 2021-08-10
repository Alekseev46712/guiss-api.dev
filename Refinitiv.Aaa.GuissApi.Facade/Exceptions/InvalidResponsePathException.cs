using System;
using System.Runtime.Serialization;

namespace Refinitiv.Aaa.GuissApi.Facade.Exceptions
{
    /// <summary>
    /// Exception indicating that response path of the attribute is invalid.
    /// </summary>
    [Serializable]
    public class InvalidResponsePathException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidResponsePathException"/> class.
        /// </summary>
        /// <param name="attributeName">Attribute name.</param>
        /// <param name="responsePath">Response path.</param>
        /// <param name="providerName">Provider name.</param>
        public InvalidResponsePathException(string attributeName, string responsePath, string providerName)
            : base($"Response path '{responsePath}' is invalid for the attribute '{attributeName}' " +
                   $"related to the '{providerName}' provider.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidResponsePathException"/> class.
        /// </summary>
        /// <param name="serializationInfo">Serialization information.</param>
        /// <param name="streamingContext">Streaming context.</param>
        protected InvalidResponsePathException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
