namespace Refinitiv.Aaa.GuissApi.Models
{
    /// <summary>
    /// Helper class used to get swagger settings from the configuration file.
    /// </summary>
    public sealed class SwaggerConfiguration
    {
        /// <summary>
        /// Gets the title of the Swagger documentation in the config file.
        /// </summary>
        /// <value>The Title.</value>
        public string Title { get; set; }

        /// <summary>
        /// Gets the version of the Swagger documentation in the config file.
        /// </summary>
        /// <value>The Version.</value>
        public string Version { get; set; }

        /// <summary>
        /// Gets the description of the Swagger documentation in the config file.
        /// </summary>
        /// <value>The Description.</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets the endpoint of the Swagger documentation in the config file.
        /// </summary>
        /// <value>The Endpoint.</value>
        public string Endpoint { get; set; }
    }
}
