using ArcadeFrontend.Enums;
using System;
using static SDL3.SDL;

namespace ArcadeFrontend.Interfaces;

public interface IApplicationWindow : ILoad, IDisposable
{
    event Action<float> Tick;
    event Action<float> Rendering;
    event Action BeforeDispose;
    event Action Resized;

    /// <summary>
    /// The SDL Window
    /// </summary>
    nint Window { get; }

    /// <summary>
    /// The SDL GPU Device
    /// </summary>
    nint Device { get; }
    nint Command { get; }
    nint Swapchain { get; }
    nint DepthTexture { get; }
    SDL_GPUColorTargetInfo ColorTargetInfo { get; }
    SDL_GPUColorTargetInfo ClearColorTargetInfo { get; }
    SDL_GPUDepthStencilTargetInfo DepthStencilTargetInfo { get; }
    SDL_GPUDepthStencilTargetInfo ClearDepthStencilTargetInfo { get; }
    public nint PointSampler { get; }
    public nint LinearSampler { get; }

    uint Width { get; }
    uint Height { get; }

    void Run();
    void Close();
}
