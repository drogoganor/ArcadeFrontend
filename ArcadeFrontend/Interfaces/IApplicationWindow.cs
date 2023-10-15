using ArcadeFrontend.Enums;
using Veldrid;
using Veldrid.Sdl2;

namespace ArcadeFrontend.Interfaces
{
    public interface IApplicationWindow : ILoad
    {
        PlatformType PlatformType { get; }

        event Action<float> Tick;
        event Action<float> Rendering;
        event Action Resized;
        event Action<KeyEvent> KeyPressed;

        uint Width { get; }
        uint Height { get; }
        Sdl2Window Window { get; }

        void Run();
        void Close();
    }
}
