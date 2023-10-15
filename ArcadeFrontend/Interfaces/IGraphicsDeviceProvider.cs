using Veldrid;

namespace ArcadeFrontend.Interfaces
{
    public interface IGraphicsDeviceProvider
    {
        GraphicsDevice GraphicsDevice { get; }
        ResourceFactory ResourceFactory { get; }
    }
}
