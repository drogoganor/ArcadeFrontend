using ArcadeFrontend.Interfaces;
using System.Text;
using System.Text.Json;

namespace ArcadeFrontend.Providers;

public abstract class SettingsProvider<TSettings> : ISettingsProvider<TSettings> where TSettings : new()
{
    protected abstract string Filename { get; }

    private readonly IFileSystem fileSystem;
    private readonly string preferredSaveDirectory;

    private TSettings settings;
    public TSettings Settings => settings;

    public SettingsProvider(
        IFileSystem fileSystem)
    {
        this.fileSystem = fileSystem;
        preferredSaveDirectory = Path.Combine(fileSystem.SettingsDirectory, Filename);

        var paths = new[]
        {
            preferredSaveDirectory,
            Path.Combine(Environment.CurrentDirectory, $@"Content/{Filename}")
        };

        foreach (var path in paths)
        {
            if (File.Exists(path))
            {
                try
                {
                    using var fs = File.OpenRead(path);
                    using var sr = new StreamReader(fs, Encoding.UTF8);
                    string content = sr.ReadToEnd();
                    settings = JsonSerializer.Deserialize<TSettings>(content);
                }
                catch (Exception)
                {
                    throw;
                }

                break;
            }
            else
            {
                Console.WriteLine($"Couldn't find settings file {Filename} at: {path}");
            }
        }

        settings ??= CreateNewSettings();
        settings ??= new TSettings();
    }

    protected abstract TSettings CreateNewSettings();

    public virtual void SaveSettings(TSettings settings)
    {
        this.settings = settings;
        SaveSettings();
    }

    public void SaveSettings()
    {
        try
        {
            fileSystem.CreateAppDataDirectory();

            if (File.Exists(preferredSaveDirectory))
            {
                File.Delete(preferredSaveDirectory);
            }

            using var fs = File.OpenWrite(preferredSaveDirectory);
            using var sw = new StreamWriter(fs, Encoding.UTF8);
            var content = JsonSerializer.Serialize(settings);
            sw.WriteLine(content);
            sw.Close();
        }
        catch (Exception)
        {
            throw;
        }
    }
}
