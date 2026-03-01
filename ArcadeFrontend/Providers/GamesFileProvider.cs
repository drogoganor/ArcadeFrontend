using ArcadeFrontend.Data.Files;
using ArcadeFrontend.Interfaces;
using System.Text.Json;

namespace ArcadeFrontend.Providers;

public class GamesFileProvider
{
    private readonly IFileSystem fileSystem;
    private readonly ManifestProvider manifestProvider;

    public Dictionary<string, GamesFile> Data { get; private set; } = new();

    public GamesFileProvider(
        IFileSystem fileSystem,
        ManifestProvider manifestProvider)
    {
        this.fileSystem = fileSystem;
        this.manifestProvider = manifestProvider;
    }

    public void Load()
    {
        var files = manifestProvider.ManifestFile.GamesFiles;
        foreach (var file in files)
        {
            var filePath = Path.Combine(fileSystem.ContentDirectory, file);
            var json = File.ReadAllText(filePath);

            try
            {
                var data = JsonSerializer.Deserialize<GamesFile>(json);
                Data.Add(data.Name, data);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }

    public void Unload()
    {
        Data.Clear();
    }
}
