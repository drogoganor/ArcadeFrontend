using ArcadeFrontend.Interfaces;

namespace ArcadeFrontend.Data
{
    public class FileSystem : IFileSystem
    {
        private const string PROJECT_NAME = "ArcadeFrontend";
        private const string DEV_PATH_OFFSET = $"..\\..\\..\\..\\{PROJECT_NAME}";

        private readonly string baseDirectory;
        public string BaseDirectory => baseDirectory;

        private readonly string devDirectory;
        public string DevDirectory => devDirectory;

        private readonly string contentDirectory;
        public string ContentDirectory => contentDirectory;

        private readonly string stagingDirectory;
        public string StagingDirectory => stagingDirectory;

        private readonly string stagingBackgroundsDirectory;
        public string StagingBackgroundsDirectory => stagingBackgroundsDirectory;

        private readonly string shaderDirectory;
        public string ShaderDirectory => shaderDirectory;

        private readonly string languagesDirectory;
        public string LanguagesDirectory => languagesDirectory;

        private readonly string appDataDirectory;
        public string AppDataDirectory => appDataDirectory;

        private readonly string appDataFolderName;
        public string AppDataFolderName => appDataFolderName;

        private readonly string dataDirectory;
        public string DataDirectory => dataDirectory;
        public string SettingsDirectory => AppDataDirectory;

        public FileSystem()
        {
            devDirectory = AppContext.BaseDirectory + DEV_PATH_OFFSET;
#if DEBUG
            baseDirectory = devDirectory;
#else
            baseDirectory = AppContext.BaseDirectory;
#endif
            contentDirectory = Path.Combine(baseDirectory, "Content");

            // Content
            shaderDirectory = Path.Combine(ContentDirectory, "shaders");
            languagesDirectory = Path.Combine(ContentDirectory, "i18n");

            // Dev
            stagingDirectory = Path.Combine(devDirectory, "ContentStaging");
            stagingBackgroundsDirectory = Path.Combine(stagingDirectory, "backgrounds");

            // App Data
            var localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            appDataDirectory = Path.Combine(localAppDataPath, PROJECT_NAME);
            dataDirectory = Path.Combine(appDataDirectory, "data");
        }

        public void CreateAppDataDirectory()
        {
            if (!Directory.Exists(appDataDirectory))
            {
                try
                {
                    Directory.CreateDirectory(appDataDirectory);
                    Directory.CreateDirectory(dataDirectory);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}
