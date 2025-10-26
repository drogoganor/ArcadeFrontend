namespace ArcadeFrontend.Interfaces
{
    public interface IFileSystem
    {
        string BaseDirectory { get; }
        string ContentDirectory { get; }
        string DevDirectory { get; }
        string ShaderDirectory { get; }
        string LanguagesDirectory { get; }
        string StagingDirectory { get; }
        string BackgroundsDirectory { get; }
        string AppDataFolderName { get; }
        string AppDataDirectory { get; } // System directory + AppDataFolderName
        string DataDirectory { get; }
        string SettingsDirectory { get; }

        void CreateAppDataDirectory();
    }
}
