using ArcadeFrontend.Interfaces;

namespace ArcadeFrontend.Data
{
    public class FileSystem : IFileSystem
    {
        private const string DEV_PATH_OFFSET = "..\\..\\..\\..\\ArcadeFrontend";

        private readonly string baseDirectory;
        public string BaseDirectory => baseDirectory;

        private readonly string devDirectory;
        public string DevDirectory => devDirectory;

        private readonly string contentDirectory;
        public string ContentDirectory => contentDirectory;

        private readonly string stagingDirectory;
        public string StagingDirectory => stagingDirectory;

        private readonly string stagingJsonDirectory;
        public string StagingJsonDirectory => stagingJsonDirectory;

        private readonly string stagingSpriteJsonDirectory;
        public string StagingSpriteJsonDirectory => stagingSpriteJsonDirectory;

        private readonly string stagingSpriteAssetsDirectory;
        public string StagingSpriteAssetsDirectory => stagingSpriteAssetsDirectory;

        private readonly string mapDirectory;
        public string MapDirectory => mapDirectory;

        private readonly string spriteDirectory;
        public string SpriteDirectory => spriteDirectory;

        private readonly string blocksDirectory;
        public string BlocksDirectory => blocksDirectory;

        private readonly string soundsDirectory;
        public string SoundsDirectory => soundsDirectory;

        private readonly string textureDirectory;
        public string TextureDirectory => textureDirectory;

        private readonly string skyboxDirectory;
        public string SkyboxDirectory => skyboxDirectory;

        private readonly string shaderDirectory;
        public string ShaderDirectory => shaderDirectory;

        private readonly string languagesDirectory;
        public string LanguagesDirectory => languagesDirectory;

        private readonly string appDataDirectory;
        public string AppDataDirectory => appDataDirectory;

        private readonly string appDataFolderName;
        public string AppDataFolderName => appDataFolderName;

        private readonly string saveGamesDirectory;
        public string SaveGamesDirectory => saveGamesDirectory;

        private readonly string screenshotsDirectory;
        public string ScreenshotsDirectory => screenshotsDirectory;

        private readonly string tempDirectory;
        public string TempDirectory => tempDirectory;
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
            mapDirectory = Path.Combine(ContentDirectory, "maps");
            textureDirectory = Path.Combine(ContentDirectory, "textures");
            soundsDirectory = Path.Combine(ContentDirectory, "sounds");
            skyboxDirectory = Path.Combine(ContentDirectory, "skybox");
            spriteDirectory = Path.Combine(ContentDirectory, "sprites");
            blocksDirectory = Path.Combine(ContentDirectory, "blocks");
            shaderDirectory = Path.Combine(ContentDirectory, "shaders");
            languagesDirectory = Path.Combine(ContentDirectory, "i18n");

            // Dev
            stagingDirectory = Path.Combine(devDirectory, "ContentStaging");
            stagingJsonDirectory = Path.Combine(stagingDirectory, "json");
            stagingSpriteJsonDirectory = Path.Combine(stagingDirectory, "json", "sprites");
            stagingSpriteAssetsDirectory = Path.Combine(stagingDirectory, "assets", "sprites");

            // App Data
            appDataFolderName = "ArcadeFrontend";
            var localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            appDataDirectory = Path.Combine(localAppDataPath, appDataFolderName);
            saveGamesDirectory = Path.Combine(appDataDirectory, "saves");
            screenshotsDirectory = Path.Combine(appDataDirectory, "screenshots");
            tempDirectory = Path.Combine(appDataDirectory, "temp");
        }

        public void CreateAppDataDirectory()
        {
            if (!Directory.Exists(appDataDirectory))
            {
                try
                {
                    Directory.CreateDirectory(appDataDirectory);
                    Directory.CreateDirectory(saveGamesDirectory);
                    Directory.CreateDirectory(tempDirectory);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}
