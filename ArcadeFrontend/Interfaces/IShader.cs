using Veldrid;

namespace ArcadeFrontend.Interfaces
{
    public interface IShader
    {
        CommandList CommandList { get; }
        Pipeline Pipeline { get; }
        DeviceBuffer ProjectionBuffer { get; }
        DeviceBuffer ViewBuffer { get; }
        DeviceBuffer WorldBuffer { get; }
        ResourceSet ProjectionViewSet { get; }
        ResourceSet WorldTextureSet { get; }
    }
}
