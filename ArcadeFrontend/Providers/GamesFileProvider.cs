using ArcadeFrontend.Data.Files;
using ArcadeFrontend.Interfaces;
using System.Text.Json;

namespace ArcadeFrontend.Providers
{
    public class GamesFileProvider
    {
        private readonly IFileSystem fileSystem;
        private readonly ManifestProvider manifestProvider;

        private GamesFile data;
        public GamesFile Data => data;

        public GamesFileProvider(
            IFileSystem fileSystem,
            ManifestProvider manifestProvider)
        {
            this.fileSystem = fileSystem;
            this.manifestProvider = manifestProvider;
        }

        public void Load()
        {
            var filePath = Path.Combine(fileSystem.ContentDirectory, manifestProvider.ManifestFile.GamesFile);
            var json = File.ReadAllText(filePath);

            try
            {
                data = JsonSerializer.Deserialize<GamesFile>(json);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Unload()
        {
            data = null;
        }
    }
}
