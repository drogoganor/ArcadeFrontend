using ArcadeFrontend.Data.Files;
using ArcadeFrontend.Enums;
using ArcadeFrontend.Interfaces;

namespace ArcadeFrontend.Providers;

public class FrontendSettingsProvider : SettingsProvider<SettingsFile>
{
    private const string SETTINGS_FILENAME = "settings.json";

    protected override string Filename => SETTINGS_FILENAME;

    protected override SettingsFile CreateNewSettings()
    {
        return new SettingsFile();
    }

    public FrontendSettingsProvider(
        IFileSystem fileSystem) : base(fileSystem)
    {
        CacheInputEnums();
    }

    public override void SaveSettings(SettingsFile settings)
    {
        base.SaveSettings(settings);
        CacheInputEnums();
    }

    public void CacheInputEnums()
    {
        var settings = Settings.Input;

        // Cache key and mouse button enum values
        foreach (var binding in settings.Bindings)
        {
            var inputBind = binding.Value;
            if (inputBind.InputType == InputType.Key)
            {
                var parsedKey = (SdlKey)Enum.Parse(typeof(SdlKey), inputBind.Input);
                inputBind.Key = parsedKey;
            }
            else if (inputBind.InputType == InputType.Mouse)
            {
                var parsedMouse = (SdlMouseButton)Enum.Parse(typeof(SdlMouseButton), inputBind.Input);
                inputBind.MouseButton = parsedMouse;
            }
        }
    }
}
