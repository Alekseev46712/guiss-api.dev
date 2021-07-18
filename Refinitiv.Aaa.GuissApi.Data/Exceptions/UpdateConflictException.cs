using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Refinitiv.Aaa.GuissApi.Data.Exceptions
{
    /// <summary>
    /// Exception indicating that an item has been simultaneously updated by another user.
    /// </summary>
    [Serializable]
    public class UpdateConflictException : Exception
    {
        private const string ErrorMessage = "The item has been updated by another user.";

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateConflictException"/> class.
        /// </summary>
        public UpdateConflictException()
            : base(ErrorMessage)
        {
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref="UpdateConflictException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public UpdateConflictException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref="UpdateConflictException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">Exception.</param>
        public UpdateConflictException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref="UpdateConflictException"/> class.
        /// </summary>
        /// <param name="innerException">Exception.</param>
        public UpdateConflictException(Exception innerException)
            : base(ErrorMessage, innerException)
        {
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref="UpdateConflictException"/> class.
        /// </summary>
        /// <param name="info">SerializationInfo.</param>
        /// <param name="context">Context.</param>
        [ExcludeFromCodeCoverage]
        protected UpdateConflictException(SerializationInfo info, StreamingContext context)
            : base(ErrorMessage)
        {
        }
    }
}
