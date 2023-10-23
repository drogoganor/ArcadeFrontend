using ArcadeFrontend.Interfaces;

namespace ArcadeFrontend.Providers
{
    /// <summary>
    /// Loads actual game data from the mod manifest.
    /// Required so that block and sprite editors do not automatically load the default block info.
    /// </summary>
    public class FileLoadProvider : ILoad
    {
        private readonly IFileSystem fileSystem;
        private readonly ManifestProvider manifestProvider;
        private readonly GamesFileProvider gamesFileProvider;

        public FileLoadProvider(
            IFileSystem fileSystem,
            ManifestProvider manifestProvider,
            GamesFileProvider gamesFileProvider
            )
        {
            this.fileSystem = fileSystem;
            this.manifestProvider = manifestProvider;
            this.gamesFileProvider = gamesFileProvider;
        }

        public void Load()
        {
            gamesFileProvider.Load();
        }

        public void Unload()
        {
            gamesFileProvider.Unload();
        }
    }
}
