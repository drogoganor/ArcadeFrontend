using Serilog;
using ArcadeFrontend.Interfaces;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace ArcadeFrontend.Providers;

public abstract class SettingsProvider<TSettings> : ISettingsProvider<TSettings> where TSettings : new()
{
    protected abstract string Filename { get; }

    private readonly ILogger logger;
    private readonly IFileSystem fileSystem;
    private readonly string userSettingsPath;

    private TSettings settings;
    public TSettings Settings => settings;

    public SettingsProvider(
        ILogger logger,
        IFileSystem fileSystem)
    {
        this.logger = logger;
        this.fileSystem = fileSystem;

        fileSystem.CreateAppDataDirectory();

        userSettingsPath = Path.Combine(fileSystem.SettingsDirectory, Filename);
        var contentSettingsPath = Path.Combine(Environment.CurrentDirectory, $@"Content/{Filename}");

        if (Debugger.IsAttached)
        {
            // This is a hack
            contentSettingsPath = Path.Combine(Environment.CurrentDirectory, $@"../../../../ArcadeFrontend/Content/{Filename}");
        }

        if (!File.Exists(userSettingsPath) && File.Exists(contentSettingsPath))
        {
            //logger.Warning($"Couldn't find user directory settings file at: {userSettingsPath}");
            try
            {
                File.Copy(contentSettingsPath, userSettingsPath);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Couldn't copy default settings file to user directory at {path}", userSettingsPath);
            }
        }

        if (TryLoadSettingsFile(userSettingsPath))
        {
            return;
        }

        if (TryLoadSettingsFile(contentSettingsPath))
        {
            return;
        }

        settings ??= CreateNewSettings();
        settings ??= new TSettings();

        if (!File.Exists(userSettingsPath))
        {
            var settingsJson = JsonSerializer.Serialize(settings);
            try
            {
                File.WriteAllText(userSettingsPath, settingsJson);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Couldn't write default settings file to user directory at {path}", userSettingsPath);
            }
        }
    }

    private bool TryLoadSettingsFile(string path)
    {
        if (!File.Exists(path))
            return false;

        //logger.Information("Loading settings from {path}", path);
        try
        {
            using var fs = File.OpenRead(path);
            using var sr = new StreamReader(fs, Encoding.UTF8);
            string content = sr.ReadToEnd();
            settings = JsonSerializer.Deserialize<TSettings>(content);
            return true;
        }
        catch (Exception)
        {
            throw;
        }
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

            if (File.Exists(userSettingsPath))
            {
                File.Delete(userSettingsPath);
            }

            using var fs = File.OpenWrite(userSettingsPath);
            using var sw = new StreamWriter(fs, Encoding.UTF8);
            var content = JsonSerializer.Serialize(settings);
            sw.WriteLine(content);
            sw.Close();
        }
        catch (Exception ex)
        {
            logger.Error(ex, $"Couldn't write settings file {Filename} at: {userSettingsPath}");
            throw;
        }
    }
}
