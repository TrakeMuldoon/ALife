using ALife.Core.Utility.IO;
using System;
using System.IO;

namespace ALife.Core
{
    /// <summary>
    /// A settings class for the application.
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// The agent save directory name
        /// </summary>
        public static readonly string AGENT_SAVE_DIRECTORY_NAME = "ExportedAgents";

        /// <summary>
        /// The settings file name
        /// </summary>
        public static readonly string SETTINGS_FILE_NAME = "settings.json";

        /// <summary>
        /// The world save directory name
        /// </summary>
        public static readonly string WORLD_SAVE_DIRECTORY_NAME = "Saves";

        /// <summary>
        /// The world save file extension
        /// </summary>
        public static readonly string WORLD_SAVE_FILE_EXTENSION = ".world";

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        /// <param name="applicationName">Name of the application.</param>
        public Settings(string applicationName) : this(applicationName, Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        /// <param name="applicationName">Name of the application.</param>
        /// <param name="rootUserDirectory">The root user directory.</param>
        public Settings(string applicationName, string rootUserDirectory)
        {
            ApplicationName = applicationName;
            ApplicationSafeName = applicationName.Replace(" ", "_");
            RootUserDirectory = rootUserDirectory;
            BaseApplicationDirectory = Path.Combine(rootUserDirectory, ApplicationSafeName);

            SettingsFilePath = Path.Combine(BaseApplicationDirectory, SETTINGS_FILE_NAME);
            WorldSaveDirectoryPath = Path.Combine(BaseApplicationDirectory, WORLD_SAVE_DIRECTORY_NAME);
            AgentExportDirectoryPath = Path.Combine(BaseApplicationDirectory, AGENT_SAVE_DIRECTORY_NAME);
        }

        /// <summary>
        /// Gets the agent export directory path.
        /// </summary>
        /// <value>The agent export directory path.</value>
        public string AgentExportDirectoryPath { get; }

        /// <summary>
        /// Gets the name of the application.
        /// </summary>
        /// <value>The name of the application.</value>
        public string ApplicationName { get; }

        /// <summary>
        /// Gets the safe name of the application.
        /// </summary>
        /// <value>The safe name of the application.</value>
        public string ApplicationSafeName { get; }

        /// <summary>
        /// Gets the base application directory.
        /// </summary>
        /// <value>The base application directory.</value>
        public string BaseApplicationDirectory { get; }

        /// <summary>
        /// Gets the root user directory.
        /// </summary>
        /// <value>The root user directory.</value>
        public string RootUserDirectory { get; }

        /// <summary>
        /// Gets the settings file path.
        /// </summary>
        /// <value>The settings file path.</value>
        public string SettingsFilePath { get; }

        /// <summary>
        /// Gets the world save directory path.
        /// </summary>
        /// <value>The world save directory path.</value>
        public string WorldSaveDirectoryPath { get; }

        public void InitializeDirectories()
        {
            IOHelpers.CreateDirectoryIfNotExists(BaseApplicationDirectory);
            IOHelpers.CreateDirectoryIfNotExists(WorldSaveDirectoryPath);
            IOHelpers.CreateDirectoryIfNotExists(AgentExportDirectoryPath);
        }
    }
}
