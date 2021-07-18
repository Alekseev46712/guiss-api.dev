namespace Refinitiv.Aaa.GuissApi.Facade.Interfaces
{
    /// <summary>
    /// Represents the AppSettings section of the configuration file.
    /// </summary>
    public interface IAppSettingsConfiguration
    {
        /// <summary>
        /// Gets the ApplicationId from the configuration file.
        /// </summary>
        /// <value>The ID of the application.</value>
        string ApplicationId { get; }

        /// <summary>
        /// Gets the default query limit set in configuration.
        /// </summary>
        /// <value>Int value containing the DefaultQueryLimit.</value>
        int DefaultQueryLimit { get; }
    }
}
