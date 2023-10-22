namespace ArcadeFrontend.Interfaces
{
    public interface ISettingsProvider<TSettings>
    {
        TSettings Settings { get; }
        void SaveSettings();
    }
}
