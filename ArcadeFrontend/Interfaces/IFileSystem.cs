namespace ArcadeFrontend.Interfaces
{
    public interface IFileSystem
    {
        string BaseDirectory { get; }
        string ContentDirectory { get; }
        string MapDirectory { get; }
        string DevDirectory { get; }
        string SoundsDirectory { get; }
        string TextureDirectory { get; }
        string SkyboxDirectory { get; }
        string SpriteDirectory { get; }
        string BlocksDirectory { get; }
        string ShaderDirectory { get; }
        string LanguagesDirectory { get; }
        string StagingDirectory { get; }
        string StagingJsonDirectory { get; }
        string StagingSpriteJsonDirectory { get; }
        string StagingSpriteAssetsDirectory { get; }
        string AppDataFolderName { get; }
        string AppDataDirectory { get; } // System directory + AppDataFolderName
        string SaveGamesDirectory { get; }
        string ScreenshotsDirectory { get; }
        string SettingsDirectory { get; }
        string TempDirectory { get; }

        void CreateAppDataDirectory();
    }
}
