using ArcadeFrontend.Data.Files;
using ArcadeFrontend.Interfaces;
using System.Text.Json;

namespace ArcadeFrontend.Providers
{
    public class ManifestProvider
    {
        private readonly ManifestFile manifestFile;

        public ManifestFile ManifestFile => manifestFile;

        public ManifestProvider(IFileSystem fileSystem)
        {
            var filePath = Path.Combine(fileSystem.ContentDirectory, "manifest.json");
            var manifestJson = File.ReadAllText(filePath);
            manifestFile = JsonSerializer.Deserialize<ManifestFile>(manifestJson);
        }
    }
}
