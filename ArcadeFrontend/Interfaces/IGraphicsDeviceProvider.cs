using Veldrid;

namespace ArcadeFrontend.Interfaces;

public interface IGraphicsDeviceProvider : ILoad
{
    GraphicsDevice GraphicsDevice { get; }
    ResourceFactory ResourceFactory { get; }
    Framebuffer Framebuffer { get; }

    void ResetFramebuffer();
}
