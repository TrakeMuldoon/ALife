using ALife.Core;

namespace ALife.Tests.ROOT
{
    /// <summary>
    /// Tests for the Settings class.
    /// </summary>
    internal class TestJsonHelpers
    {
        string _applicationName = "Test Application";
        string _applicationSafeName = "Test_Application";
        Settings _settings;

        [SetUp]
        public void SetUp()
        {
            _settings = new Settings(_applicationName);
        }

        [Test]
        public void TestSettingsNormal()
        {
            Assert.AreEqual(_applicationName, _settings.ApplicationName);
            Assert.AreEqual(_applicationSafeName, _settings.ApplicationSafeName);

            // Root Dir
            var expectedRootDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            Assert.AreEqual(expectedRootDirectory, _settings.RootUserDirectory);

            // Base Dir
            var expectedBaseDirectory = Path.Combine(expectedRootDirectory, _applicationSafeName);
            Assert.AreEqual(expectedBaseDirectory, _settings.BaseApplicationDirectory);

            // Setting path
            var settingsPath = Path.Combine(expectedBaseDirectory, Settings.SETTINGS_FILE_NAME);
            Assert.AreEqual(settingsPath, _settings.SettingsFilePath);

            // World Save Dir
            var worldsPath = Path.Combine(expectedBaseDirectory, Settings.WORLD_SAVE_DIRECTORY_NAME);
            Assert.AreEqual(worldsPath, _settings.WorldSaveDirectoryPath);

            // Agent Export Dir
            var agentsPath = Path.Combine(expectedBaseDirectory, Settings.AGENT_SAVE_DIRECTORY_NAME);
            Assert.AreEqual(agentsPath, _settings.AgentExportDirectoryPath);
        }
    }
}
