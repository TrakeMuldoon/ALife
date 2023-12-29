using ALife.Core;

namespace ALife.Tests
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
            Assert.That(_settings.ApplicationName, Is.EqualTo(_applicationName));
            Assert.That(_settings.ApplicationSafeName, Is.EqualTo(_applicationSafeName));

            // Root Dir
            var expectedRootDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            Assert.That(_settings.RootUserDirectory, Is.EqualTo(expectedRootDirectory));

            // Base Dir
            var expectedBaseDirectory = Path.Combine(expectedRootDirectory, _applicationSafeName);
            Assert.That(_settings.BaseApplicationDirectory, Is.EqualTo(expectedBaseDirectory));

            // Setting path
            var settingsPath = Path.Combine(expectedBaseDirectory, Settings.SETTINGS_FILE_NAME);
            Assert.That(_settings.SettingsFilePath, Is.EqualTo(settingsPath));

            // World Save Dir
            var worldsPath = Path.Combine(expectedBaseDirectory, Settings.WORLD_SAVE_DIRECTORY_NAME);
            Assert.That(_settings.WorldSaveDirectoryPath, Is.EqualTo(worldsPath));

            // Agent Export Dir
            var agentsPath = Path.Combine(expectedBaseDirectory, Settings.AGENT_SAVE_DIRECTORY_NAME);
            Assert.That(_settings.AgentExportDirectoryPath, Is.EqualTo(agentsPath));
        }
    }
}
