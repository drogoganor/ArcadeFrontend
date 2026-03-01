using ArcadeFrontend.Enums;
using ArcadeFrontend.Render;
using System;
using static SDL3.SDL;

namespace ArcadeFrontend.Interfaces;

public interface IApplicationWindow : ILoad, IDisposable
{
    PlatformType PlatformType { get; }

    event Action<float> Tick;
    event Action<float> Rendering;
    event Action<float> RenderingUI;
    event Action<float> PostRender;
    event Action Resized;
    //event Action<KeyEvent> KeyPressed;

    nint Window { get; }
    nint Device { get; }
    nint Command { get; }
    nint Swapchain { get; }
    nint DepthTexture { get; }
    Sdl3ImGuiRenderer ImGuiRenderer { get; }
    SDL_GPUColorTargetInfo ColorTargetInfo { get; }
    SDL_GPUColorTargetInfo ClearColorTargetInfo { get; }
    SDL_GPUDepthStencilTargetInfo DepthStencilTargetInfo { get; }
    SDL_GPUDepthStencilTargetInfo ClearDepthStencilTargetInfo { get; }
    public nint PointSampler { get; }
    public nint LinearSampler { get; }

    uint Width { get; }
    uint Height { get; }

    //Sdl2Window Window { get; }

    void Run();
    void Close();
}
