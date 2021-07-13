using System;
using System.Runtime.Serialization;

namespace Refinitiv.Aaa.GuissApi.Facade.Exceptions
{
    /// <summary>
    /// Exception indicating that the user has supplied an invalid pagination token.
    /// </summary>
    [Serializable]
    public class InvalidPaginationTokenException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidPaginationTokenException"/> class.
        /// </summary>
        public InvalidPaginationTokenException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidPaginationTokenException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public InvalidPaginationTokenException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidPaginationTokenException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The exception which caused the update conflict.</param>
        public InvalidPaginationTokenException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidPaginationTokenException"/> class.
        /// </summary>
        /// <param name="serializationInfo">Serialization information.</param>
        /// <param name="streamingContext">Streaming context.</param>
        protected InvalidPaginationTokenException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
