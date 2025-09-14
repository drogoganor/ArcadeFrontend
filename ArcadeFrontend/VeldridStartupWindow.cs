using ArcadeFrontend.Enums;
using ArcadeFrontend.Interfaces;
using ArcadeFrontend.Providers;
using System.Diagnostics;
using Veldrid;
using Veldrid.Sdl2;

namespace ArcadeFrontend;

public class VeldridStartupWindow : IApplicationWindow
{
    private readonly Sdl2WindowProvider sdl2WindowProvider;
    private readonly IGraphicsDeviceProvider graphicsDeviceProvider;
    private readonly NextTickActionProvider nextTickActionProvider;

    public event Action<float> Tick;
    public event Action<float> Rendering;
    public event Action Resized;
    public event Action<KeyEvent> KeyPressed;

    private bool windowResized = false;
    private GraphicsDevice graphicsDevice;
    private Sdl2Window window;
    public Sdl2Window Window => window;
    public uint Width => (uint)window.Width;
    public uint Height => (uint)window.Height;

    public PlatformType PlatformType => PlatformType.Desktop;

    public VeldridStartupWindow(
        Sdl2WindowProvider sdl2WindowProvider,
        IGraphicsDeviceProvider graphicsDeviceProvider,
        NextTickActionProvider nextTickActionProvider)
    {
        this.sdl2WindowProvider = sdl2WindowProvider;
        this.graphicsDeviceProvider = graphicsDeviceProvider;
        this.nextTickActionProvider = nextTickActionProvider;
    }

    public void Run()
    {
        var sw = Stopwatch.StartNew();
        var previousElapsed = sw.Elapsed.TotalSeconds;

        while (window.Exists)
        {
            double newElapsed = sw.Elapsed.TotalSeconds;
            float deltaSeconds = (float)(newElapsed - previousElapsed);

            nextTickActionProvider.Tick(deltaSeconds);

            var inputSnapshot = window.PumpEvents();
            InputTracker.UpdateFrameInput(inputSnapshot, window);

            if (window.Exists)
            {
                previousElapsed = newElapsed;
                
                if (windowResized)
                {
                    windowResized = false;
                    graphicsDevice.ResizeMainWindow((uint)window.Width, (uint)window.Height);
                    Resized?.Invoke();
                }

                Tick?.Invoke(deltaSeconds);
                Rendering?.Invoke(deltaSeconds);
            }
        }
    }

    protected void OnKeyDown(KeyEvent keyEvent)
    {
        KeyPressed?.Invoke(keyEvent);
    }

    private void HandleResize()
    {
        windowResized = true;
    }

    public void Close()
    {
        window.Close();
    }

    public void Load()
    {
        graphicsDevice = graphicsDeviceProvider.GraphicsDevice;
        window = sdl2WindowProvider.Window;
        window.Resized += HandleResize;
        window.KeyDown += OnKeyDown;
    }

    public void Unload()
    {
        window.Resized -= HandleResize;
        window.KeyDown -= OnKeyDown;
    }
}
