using System;
using Refinitiv.Aaa.Interfaces.Business;

namespace Refinitiv.Aaa.GuissApi.Facade.Models
{
    /// <summary>
    /// An object that represents a Guiss.
    /// </summary>
    public class Guiss : GuissDetails, IModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Guiss"/> class.
        /// </summary>
        public Guiss()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Guiss"/> class.
        /// </summary>
        /// <param name="details">The details.</param>
        public Guiss(GuissDetails details)
            : base(details)
        {
        }

        /// <inheritdoc />
        public string Id { get; set; }

        /// <inheritdoc />
        public long? Version { get; set; }

        /// <inheritdoc />
        public DateTimeOffset UpdatedOn { get; set; }

        /// <inheritdoc />
        public string UpdatedBy { get; set; }
    }
}
