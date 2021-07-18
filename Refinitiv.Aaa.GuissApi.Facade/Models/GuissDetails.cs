using System;

namespace Refinitiv.Aaa.GuissApi.Facade.Models
{
    /// <summary>
    /// Viewmodel for the request body that is used to create a new template.
    /// </summary>
    /// <remarks>The user is not allowed to specify the ID, timestamp or version number.</remarks>
    public class GuissDetails
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GuissDetails"/> class.
        /// </summary>
        public GuissDetails()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GuissDetails"/> class.
        /// </summary>
        /// <param name="details">GuissDetails.</param>
        public GuissDetails(GuissDetails details)
        {
            if (details == null)
            {
                throw new ArgumentNullException(nameof(details));
            }

            this.Name = details.Name;
        }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        /// <value>The Name.</value>
        public string Name { get; set; }
    }
}
