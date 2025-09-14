namespace ArcadeFrontend.Interfaces;

/// <summary>
/// ILoad items needing to be loaded once upon any app start
/// </summary>
public interface ILoadProvider
{
    void Load();
    void Unload();
}
