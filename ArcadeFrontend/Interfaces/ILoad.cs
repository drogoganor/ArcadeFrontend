namespace ArcadeFrontend.Interfaces
{
    /// <summary>
    /// Items that need to be loaded on any app start
    /// </summary>
    public interface ILoad
    {
        void Load();
        void Unload();
    }
}
