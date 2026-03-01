using ImGuiNET;
using ArcadeFrontend.Enums;
using ArcadeFrontend.Interfaces;
using ArcadeFrontend.Providers;
using ArcadeFrontend.Render;
using System;
using System.Diagnostics;
using System.Numerics;
using static SDL3.SDL;

namespace ArcadeFrontend;

public class Sdl3Window : IApplicationWindow
{
    private readonly IFileSystem fileSystem;
    private readonly FrontendSettingsProvider settingsProvider;
    private readonly ManifestProvider manifestProvider;
    private readonly NextTickActionProvider nextTickActionProvider;

    /// <summary>
    /// The SDL Window
    /// </summary>
    public nint Window { get; private set; }

    /// <summary>
    /// The SDL GPU Device
    /// </summary>
    public nint Device { get; private set; }
    public nint Command { get; private set; }
    public nint Swapchain { get; private set; }
    public nint DepthTexture { get; private set; }

    /// <summary>
    /// The ImGui Renderer Implementation
    /// </summary>
    public Sdl3ImGuiRenderer ImGuiRenderer { get; private set; }

    public uint Width { get; private set; }
    public uint Height { get; private set; }

    public SDL_GPUColorTargetInfo ColorTargetInfo { get; private set; }
    public SDL_GPUColorTargetInfo ClearColorTargetInfo { get; private set; }
    public SDL_GPUDepthStencilTargetInfo DepthStencilTargetInfo { get; private set; }
    public SDL_GPUDepthStencilTargetInfo ClearDepthStencilTargetInfo { get; private set; }
    public nint PointSampler { get; private set; }
    public nint LinearSampler { get; private set; }


    /// <summary>
    /// If the Application is Running
    /// </summary>
    private bool Running;

    private readonly Stopwatch timer = Stopwatch.StartNew();
    private TimeSpan time = TimeSpan.Zero;

    public event Action<float> Tick;
    public event Action<float> RenderingUI;
    public event Action<float> Rendering;
    public event Action<float> PostRender;
    public event Action Resized;
    //public event Action<KeyEvent> KeyPressed;

    private SdlSimpleInputSnapshot snapshot = new();

    public PlatformType PlatformType => PlatformType.Desktop;

    private bool windowResized;

    public Sdl3Window(
        IFileSystem fileSystem,
        FrontendSettingsProvider settingsProvider,
        ManifestProvider manifestProvider,
        NextTickActionProvider nextTickActionProvider)
    {
        this.fileSystem = fileSystem;
        this.manifestProvider = manifestProvider;
        this.settingsProvider = settingsProvider;
        this.nextTickActionProvider = nextTickActionProvider;
    }

    public void Load()
    {
        //window.Resized += HandleResize;
        //window.KeyDown += OnKeyDown;

        var modInfo = manifestProvider.ManifestFile;
        var settings = settingsProvider.Settings.Video;

        var debugMode = true;

        //var windowSize = settings.ScreenType == ScreenType.Windowed ? settings.WindowedSize : settings.FullscreenSize;
        //var initialState = settings.ScreenType switch
        //{
        //    ScreenType.Windowed => WindowState.Normal,
        //    ScreenType.FullscreenWindowed => WindowState.BorderlessFullScreen,
        //    ScreenType.Fullscreen => WindowState.BorderlessFullScreen,
        //    _ => WindowState.Normal,
        //};

        // Set to Vulkan!
        SDL_SetHint(SDL_HINT_GPU_DRIVER, "vulkan");

        // launch SDL
        if (!SDL_Init(SDL_InitFlags.SDL_INIT_VIDEO | SDL_InitFlags.SDL_INIT_AUDIO))
            throw new Exception($"{nameof(SDL_Init)} Failed: {SDL_GetError()}");

        SDL_SetHint(SDL_HINT_RENDER_VULKAN_DEBUG, "1");
        SDL_SetHint(SDL_HINT_RENDER_GPU_DEBUG, "1");

        // create graphics device
        Device = SDL_CreateGPUDevice(
            SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_DXIL |
            SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_MSL |
            SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_SPIRV,
            debug_mode: debugMode,
            name: null!
        );

        var width = (int)settings.WindowedSize.X;
        var height = (int)settings.WindowedSize.Y;

        Width = (uint)width;
        Height = (uint)height;

        if (Device == nint.Zero)
            throw new Exception($"{nameof(SDL_CreateGPUDevice)} Failed: {SDL_GetError()}");

        // create window
        var windowFlags = SDL_WindowFlags.SDL_WINDOW_RESIZABLE;
        Window = SDL_CreateWindow(modInfo.Name, width, height, windowFlags);
        if (Window == nint.Zero)
            throw new Exception($"{nameof(SDL_CreateWindow)} Failed: {SDL_GetError()}");

        // claim SDL GPU Window
        if (!SDL_ClaimWindowForGPUDevice(Device, Window))
            throw new Exception($"{nameof(SDL_ClaimWindowForGPUDevice)} Failed: {SDL_GetError()}");

        // create imgui context
        var context = ImGui.CreateContext();
        ImGui.SetCurrentContext(context);

        // create imgui SDL_GPU renderer
        ImGuiRenderer = new Sdl3ImGuiRenderer(manifestProvider, Device, Window, context);

        // Depth texture
        CreateDepthTexture();

        PointSampler = SDL_CreateGPUSampler(Device, new()
        {
            min_filter = SDL_GPUFilter.SDL_GPU_FILTER_NEAREST,
            mag_filter = SDL_GPUFilter.SDL_GPU_FILTER_NEAREST,
            mipmap_mode = SDL_GPUSamplerMipmapMode.SDL_GPU_SAMPLERMIPMAPMODE_NEAREST,
            address_mode_u = SDL_GPUSamplerAddressMode.SDL_GPU_SAMPLERADDRESSMODE_CLAMP_TO_EDGE,
            address_mode_v = SDL_GPUSamplerAddressMode.SDL_GPU_SAMPLERADDRESSMODE_CLAMP_TO_EDGE,
            address_mode_w = SDL_GPUSamplerAddressMode.SDL_GPU_SAMPLERADDRESSMODE_CLAMP_TO_EDGE,
        });

        LinearSampler = SDL_CreateGPUSampler(Device, new()
        {
            min_filter = SDL_GPUFilter.SDL_GPU_FILTER_LINEAR,
            mag_filter = SDL_GPUFilter.SDL_GPU_FILTER_LINEAR,
            mipmap_mode = SDL_GPUSamplerMipmapMode.SDL_GPU_SAMPLERMIPMAPMODE_LINEAR,
            address_mode_u = SDL_GPUSamplerAddressMode.SDL_GPU_SAMPLERADDRESSMODE_CLAMP_TO_EDGE,
            address_mode_v = SDL_GPUSamplerAddressMode.SDL_GPU_SAMPLERADDRESSMODE_CLAMP_TO_EDGE,
            address_mode_w = SDL_GPUSamplerAddressMode.SDL_GPU_SAMPLERADDRESSMODE_CLAMP_TO_EDGE,
        });
    }

    public void Unload()
    {
        SDL_ReleaseGPUTexture(Device, DepthTexture);

        SDL_ReleaseGPUSampler(Device, PointSampler);
        SDL_ReleaseGPUSampler(Device, LinearSampler);

        //window.Resized -= HandleResize;
        //window.KeyDown -= OnKeyDown;
    }

    ~Sdl3Window() => Dispose();

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        ImGui.DestroyContext(ImGuiRenderer.Context);
        Running = false;
        ImGuiRenderer.Dispose();
        SDL_ReleaseWindowFromGPUDevice(Device, Window);
        SDL_DestroyWindow(Window);
        SDL_DestroyGPUDevice(Device);
        SDL_Quit();
    }

    public void Run()
    {
        var sw = Stopwatch.StartNew();
        var previousElapsed = sw.Elapsed.TotalSeconds;

        Running = true;

        while (Running)
        {
            // update delta time
            var deltaSeconds = (float)(timer.Elapsed - time).TotalSeconds;
            ImGui.GetIO().DeltaTime = deltaSeconds;
            time = timer.Elapsed;

            if (windowResized)
            {
                windowResized = false;

                if (SDL_GetWindowSizeInPixels(Window, out var width, out var height))
                {
                    Width = (uint)width;
                    Height = (uint)height;
                }

                Resized?.Invoke();
            }

            nextTickActionProvider.Tick(deltaSeconds);

            if (!Running)
                break;

            // run update
            PollEvents();
            Update(deltaSeconds);
            Render(deltaSeconds);
        }
    }
    
    private void CreateDepthTexture()
    {
        if (DepthTexture != nint.Zero)
        {
            SDL_ReleaseGPUTexture(Device, DepthTexture);
            DepthTexture = nint.Zero;
        }

        DepthTexture = SDL_CreateGPUTexture(Device, new SDL_GPUTextureCreateInfo
        {
            type = SDL_GPUTextureType.SDL_GPU_TEXTURETYPE_2D,
            width = Width,
            height = Height,
            format = SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_D24_UNORM_S8_UINT, // SDL_GPU_TEXTUREFORMAT_D32_FLOAT
            layer_count_or_depth = 1,
            usage = SDL_GPUTextureUsageFlags.SDL_GPU_TEXTUREUSAGE_DEPTH_STENCIL_TARGET,
            num_levels = 1,
            //sample_count = SDL_GPUSampleCount.SDL_GPU_SAMPLECOUNT_1
        });

        DepthStencilTargetInfo = new SDL_GPUDepthStencilTargetInfo
        {
            texture = DepthTexture,
            //clear_depth = 1.0f,
            load_op = SDL_GPULoadOp.SDL_GPU_LOADOP_LOAD,
            store_op = SDL_GPUStoreOp.SDL_GPU_STOREOP_STORE,
            //clear_stencil = 0,
            stencil_load_op = SDL_GPULoadOp.SDL_GPU_LOADOP_LOAD,
            stencil_store_op = SDL_GPUStoreOp.SDL_GPU_STOREOP_STORE
        };

        ClearDepthStencilTargetInfo = new SDL_GPUDepthStencilTargetInfo
        {
            texture = DepthTexture,
            clear_depth = 1.0f,
            load_op = SDL_GPULoadOp.SDL_GPU_LOADOP_CLEAR,
            store_op = SDL_GPUStoreOp.SDL_GPU_STOREOP_STORE,
            clear_stencil = 0,
            stencil_load_op = SDL_GPULoadOp.SDL_GPU_LOADOP_CLEAR,
            stencil_store_op = SDL_GPUStoreOp.SDL_GPU_STOREOP_STORE
        };
    }

    private void PollEvents()
    {
        // toggle keyboard input
        if (ImGui.GetIO().WantTextInput && !SDL_TextInputActive(Window))
            SDL_StartTextInput(Window);
        else if (!ImGui.GetIO().WantTextInput && SDL_TextInputActive(Window))
            SDL_StopTextInput(Window);

        snapshot.Clear();

        // poll SDL events
        while (SDL_PollEvent(out var ev))
        {
            ProcessImGuiEvent(ev);

            ProcessInputEvent(ev);

            switch ((SDL_EventType)ev.type)
            {
                case SDL_EventType.SDL_EVENT_WINDOW_RESIZED:
                    HandleResize();
                    break;
                case SDL_EventType.SDL_EVENT_WINDOW_CLOSE_REQUESTED:
                case SDL_EventType.SDL_EVENT_QUIT:
                    Running = false;
                    break;
            }
        }

        Sdl3InputTracker.UpdateFrameInput(snapshot);
    }

    private void Update(float deltaSeconds)
    {
        Tick?.Invoke(deltaSeconds);

        ImGuiRenderer.NewFrame();
        ImGui.NewFrame();

        RenderingUI?.Invoke(deltaSeconds);

        ImGui.EndFrame();
    }

    private void Render(float deltaSeconds)
    {
        var clearColor = new SDL_FColor() { r = 0.14f, g = 0.23f, b = 0.14f, a = 1.0f };

        Swapchain = nint.Zero;
        Command = SDL_AcquireGPUCommandBuffer(Device);

        if (!SDL_WaitAndAcquireGPUSwapchainTexture(Command, Window, out var swapchain, out var width, out var height))
        {
            Console.WriteLine($"{nameof(SDL_WaitAndAcquireGPUSwapchainTexture)} failed: {SDL_GetError()}");
        }

        Swapchain = swapchain;
        Width = width;
        Height = height;

        ColorTargetInfo = new SDL_GPUColorTargetInfo()
        {
            texture = Swapchain,
            clear_color = default,
            load_op = SDL_GPULoadOp.SDL_GPU_LOADOP_LOAD,
            store_op = SDL_GPUStoreOp.SDL_GPU_STOREOP_STORE,
        };

        ClearColorTargetInfo = new SDL_GPUColorTargetInfo()
        {
            texture = Swapchain,
            clear_color = clearColor,
            load_op = SDL_GPULoadOp.SDL_GPU_LOADOP_CLEAR,
            store_op = SDL_GPUStoreOp.SDL_GPU_STOREOP_STORE,
        };

        Rendering?.Invoke(deltaSeconds);

        ImGuiRenderer.Render(Command, Swapchain, (int)Width, (int)Height, null);

        PostRender?.Invoke(deltaSeconds);

        SDL_SubmitGPUCommandBuffer(Command);
    }

    private void ProcessInputEvent(SDL_Event ev)
    {
        switch ((SDL_EventType)ev.type)
        {
            // mouse input:

            case SDL_EventType.SDL_EVENT_MOUSE_MOTION:
                snapshot.MousePosition = new Vector2(ev.motion.x, ev.motion.y);
                break;

            case SDL_EventType.SDL_EVENT_MOUSE_BUTTON_DOWN:
            case SDL_EventType.SDL_EVENT_MOUSE_BUTTON_UP:
                ProcessMouseEvent(ev.button.down, ev.button.button);
                break;

            case SDL_EventType.SDL_EVENT_MOUSE_WHEEL:
                snapshot.WheelDelta = ev.wheel.integer_y;
                break;

            // keyboard input:
            case SDL_EventType.SDL_EVENT_KEY_DOWN:
                ProcessKeyEvent(true, ev.key);
                break;

            case SDL_EventType.SDL_EVENT_KEY_UP:
                ProcessKeyEvent(false, ev.key);
                break;

            case SDL_EventType.SDL_EVENT_TEXT_INPUT:
                break;
        }
    }

    private void ProcessKeyEvent(bool down, SDL_KeyboardEvent key)
    {
        var translatedKey = key.scancode switch
        {
            SDL_Scancode.SDL_SCANCODE_GRAVE => SdlKey.Grave,
            SDL_Scancode.SDL_SCANCODE_TAB => SdlKey.Tab,
            SDL_Scancode.SDL_SCANCODE_CAPSLOCK => SdlKey.CapsLock,
            SDL_Scancode.SDL_SCANCODE_BACKSPACE => SdlKey.BackSpace,
            SDL_Scancode.SDL_SCANCODE_PERIOD => SdlKey.Period,
            SDL_Scancode.SDL_SCANCODE_COMMA => SdlKey.Comma,
            SDL_Scancode.SDL_SCANCODE_SLASH => SdlKey.Slash,
            SDL_Scancode.SDL_SCANCODE_SEMICOLON => SdlKey.Semicolon,
            SDL_Scancode.SDL_SCANCODE_APOSTROPHE => SdlKey.Quote,
            SDL_Scancode.SDL_SCANCODE_LEFTBRACKET => SdlKey.BracketLeft,
            SDL_Scancode.SDL_SCANCODE_RIGHTBRACKET => SdlKey.BracketRight,
            SDL_Scancode.SDL_SCANCODE_MINUS => SdlKey.Minus,
            SDL_Scancode.SDL_SCANCODE_EQUALS => SdlKey.Plus,

            SDL_Scancode.SDL_SCANCODE_A => SdlKey.A,
            SDL_Scancode.SDL_SCANCODE_B => SdlKey.B,
            SDL_Scancode.SDL_SCANCODE_C => SdlKey.C,
            SDL_Scancode.SDL_SCANCODE_D => SdlKey.D,
            SDL_Scancode.SDL_SCANCODE_E => SdlKey.E,
            SDL_Scancode.SDL_SCANCODE_F => SdlKey.F,
            SDL_Scancode.SDL_SCANCODE_G => SdlKey.G,
            SDL_Scancode.SDL_SCANCODE_H => SdlKey.H,
            SDL_Scancode.SDL_SCANCODE_I => SdlKey.I,
            SDL_Scancode.SDL_SCANCODE_J => SdlKey.J,
            SDL_Scancode.SDL_SCANCODE_K => SdlKey.K,
            SDL_Scancode.SDL_SCANCODE_L => SdlKey.L,
            SDL_Scancode.SDL_SCANCODE_M => SdlKey.M,
            SDL_Scancode.SDL_SCANCODE_N => SdlKey.N,
            SDL_Scancode.SDL_SCANCODE_O => SdlKey.O,
            SDL_Scancode.SDL_SCANCODE_P => SdlKey.P,
            SDL_Scancode.SDL_SCANCODE_Q => SdlKey.Q,
            SDL_Scancode.SDL_SCANCODE_R => SdlKey.R,
            SDL_Scancode.SDL_SCANCODE_S => SdlKey.S,
            SDL_Scancode.SDL_SCANCODE_T => SdlKey.T,
            SDL_Scancode.SDL_SCANCODE_U => SdlKey.U,
            SDL_Scancode.SDL_SCANCODE_V => SdlKey.V,
            SDL_Scancode.SDL_SCANCODE_W => SdlKey.W,
            SDL_Scancode.SDL_SCANCODE_X => SdlKey.X,
            SDL_Scancode.SDL_SCANCODE_Y => SdlKey.Y,
            SDL_Scancode.SDL_SCANCODE_Z => SdlKey.Z,

            SDL_Scancode.SDL_SCANCODE_1 => SdlKey.Number1,
            SDL_Scancode.SDL_SCANCODE_2 => SdlKey.Number2,
            SDL_Scancode.SDL_SCANCODE_3 => SdlKey.Number3,
            SDL_Scancode.SDL_SCANCODE_4 => SdlKey.Number4,
            SDL_Scancode.SDL_SCANCODE_5 => SdlKey.Number5,
            SDL_Scancode.SDL_SCANCODE_6 => SdlKey.Number6,
            SDL_Scancode.SDL_SCANCODE_7 => SdlKey.Number7,
            SDL_Scancode.SDL_SCANCODE_8 => SdlKey.Number8,
            SDL_Scancode.SDL_SCANCODE_9 => SdlKey.Number9,
            SDL_Scancode.SDL_SCANCODE_0 => SdlKey.Number0,

            SDL_Scancode.SDL_SCANCODE_F1 => SdlKey.F1,
            SDL_Scancode.SDL_SCANCODE_F2 => SdlKey.F2,
            SDL_Scancode.SDL_SCANCODE_F3 => SdlKey.F3,
            SDL_Scancode.SDL_SCANCODE_F4 => SdlKey.F4,
            SDL_Scancode.SDL_SCANCODE_F5 => SdlKey.F5,
            SDL_Scancode.SDL_SCANCODE_F6 => SdlKey.F6,
            SDL_Scancode.SDL_SCANCODE_F7 => SdlKey.F7,
            SDL_Scancode.SDL_SCANCODE_F8 => SdlKey.F8,
            SDL_Scancode.SDL_SCANCODE_F9 => SdlKey.F9,
            SDL_Scancode.SDL_SCANCODE_F10 => SdlKey.F10,
            SDL_Scancode.SDL_SCANCODE_F11 => SdlKey.F11,
            SDL_Scancode.SDL_SCANCODE_F12 => SdlKey.F12,

            SDL_Scancode.SDL_SCANCODE_ESCAPE => SdlKey.Escape,
            SDL_Scancode.SDL_SCANCODE_RETURN => SdlKey.Enter,
            SDL_Scancode.SDL_SCANCODE_SPACE => SdlKey.Space,
            SDL_Scancode.SDL_SCANCODE_DELETE => SdlKey.Delete,
            SDL_Scancode.SDL_SCANCODE_UP => SdlKey.Up,
            SDL_Scancode.SDL_SCANCODE_DOWN => SdlKey.Down,
            SDL_Scancode.SDL_SCANCODE_LEFT => SdlKey.Left,
            SDL_Scancode.SDL_SCANCODE_RIGHT => SdlKey.Right,

            SDL_Scancode.SDL_SCANCODE_LCTRL => SdlKey.ControlLeft,
            SDL_Scancode.SDL_SCANCODE_LSHIFT => SdlKey.ShiftLeft,
            SDL_Scancode.SDL_SCANCODE_LALT => SdlKey.AltLeft,

            _ => SdlKey.Unknown
        };

        var keyEvent = new SdlKeyEvent(translatedKey, down, new SdlModifierKeys());
        snapshot.KeyEventsList.Add(keyEvent);
    }

    private void ProcessMouseEvent(bool down, byte button)
    {
        var transatedButton = button switch
        {
            1 => SdlMouseButton.Left,
            3 => SdlMouseButton.Right,
            _ => SdlMouseButton.LastButton
        };

        var mouseEvent = new SdlMouseEvent(transatedButton, down);
        snapshot.MouseEventsList.Add(mouseEvent);
    }

    /// <summary>
    /// Allow ImGui to process an SDL Event
    /// </summary>
    private unsafe void ProcessImGuiEvent(SDL_Event ev)
    {
        var io = ImGui.GetIO();

        switch ((SDL_EventType)ev.type)
        {
            // mouse input:

            case SDL_EventType.SDL_EVENT_MOUSE_MOTION:
                io.MousePos = new Vector2(ev.motion.x, ev.motion.y) / ImGuiRenderer.Scale;
                break;

            case SDL_EventType.SDL_EVENT_MOUSE_BUTTON_DOWN:
            case SDL_EventType.SDL_EVENT_MOUSE_BUTTON_UP:
                io.AddMouseButtonEvent(GetImGuiMouseButton(ev.button.button), ev.button.down);
                break;

            case SDL_EventType.SDL_EVENT_MOUSE_WHEEL:
                io.AddMouseWheelEvent(ev.wheel.x, ev.wheel.y);
                break;

            // keyboard input:
            case SDL_EventType.SDL_EVENT_KEY_DOWN:
            case SDL_EventType.SDL_EVENT_KEY_UP:
                io.AddKeyEvent(ImGuiKey.ModCtrl, (ev.key.mod & SDL_Keymod.SDL_KMOD_CTRL) != 0);
                io.AddKeyEvent(ImGuiKey.ModShift, (ev.key.mod & SDL_Keymod.SDL_KMOD_SHIFT) != 0);
                io.AddKeyEvent(ImGuiKey.ModAlt, (ev.key.mod & SDL_Keymod.SDL_KMOD_ALT) != 0);
                io.AddKeyEvent(ImGuiKey.ModSuper, (ev.key.mod & SDL_Keymod.SDL_KMOD_GUI) != 0);
                io.AddKeyEvent(GetImGuiKey((SDL_Keycode)ev.key.key, ev.key.scancode), ev.key.down);
                break;

            case SDL_EventType.SDL_EVENT_TEXT_INPUT:
                ImGuiNative.ImGuiIO_AddInputCharactersUTF8(io.NativePtr, ev.text.text);
                break;
        }
    }

    private static int GetImGuiMouseButton(int sdlButton) => sdlButton switch
    {
        1 => 0, // left
        2 => 2, // middle
        3 => 1, // right
        _ => 0,
    };

    private static ImGuiKey GetImGuiKey(SDL_Keycode keycode, SDL_Scancode scancode)
    {
        switch (scancode)
        {
            case SDL_Scancode.SDL_SCANCODE_KP_0: return ImGuiKey.Keypad0;
            case SDL_Scancode.SDL_SCANCODE_KP_1: return ImGuiKey.Keypad1;
            case SDL_Scancode.SDL_SCANCODE_KP_2: return ImGuiKey.Keypad2;
            case SDL_Scancode.SDL_SCANCODE_KP_3: return ImGuiKey.Keypad3;
            case SDL_Scancode.SDL_SCANCODE_KP_4: return ImGuiKey.Keypad4;
            case SDL_Scancode.SDL_SCANCODE_KP_5: return ImGuiKey.Keypad5;
            case SDL_Scancode.SDL_SCANCODE_KP_6: return ImGuiKey.Keypad6;
            case SDL_Scancode.SDL_SCANCODE_KP_7: return ImGuiKey.Keypad7;
            case SDL_Scancode.SDL_SCANCODE_KP_8: return ImGuiKey.Keypad8;
            case SDL_Scancode.SDL_SCANCODE_KP_9: return ImGuiKey.Keypad9;
            case SDL_Scancode.SDL_SCANCODE_KP_PERIOD: return ImGuiKey.KeypadDecimal;
            case SDL_Scancode.SDL_SCANCODE_KP_DIVIDE: return ImGuiKey.KeypadDivide;
            case SDL_Scancode.SDL_SCANCODE_KP_MULTIPLY: return ImGuiKey.KeypadMultiply;
            case SDL_Scancode.SDL_SCANCODE_KP_MINUS: return ImGuiKey.KeypadSubtract;
            case SDL_Scancode.SDL_SCANCODE_KP_PLUS: return ImGuiKey.KeypadAdd;
            case SDL_Scancode.SDL_SCANCODE_KP_ENTER: return ImGuiKey.KeypadEnter;
            case SDL_Scancode.SDL_SCANCODE_KP_EQUALS: return ImGuiKey.KeypadEqual;
            default: break;
        }
        switch (keycode)
        {
            case SDL_Keycode.SDLK_TAB: return ImGuiKey.Tab;
            case SDL_Keycode.SDLK_LEFT: return ImGuiKey.LeftArrow;
            case SDL_Keycode.SDLK_RIGHT: return ImGuiKey.RightArrow;
            case SDL_Keycode.SDLK_UP: return ImGuiKey.UpArrow;
            case SDL_Keycode.SDLK_DOWN: return ImGuiKey.DownArrow;
            case SDL_Keycode.SDLK_PAGEUP: return ImGuiKey.PageUp;
            case SDL_Keycode.SDLK_PAGEDOWN: return ImGuiKey.PageDown;
            case SDL_Keycode.SDLK_HOME: return ImGuiKey.Home;
            case SDL_Keycode.SDLK_END: return ImGuiKey.End;
            case SDL_Keycode.SDLK_INSERT: return ImGuiKey.Insert;
            case SDL_Keycode.SDLK_DELETE: return ImGuiKey.Delete;
            case SDL_Keycode.SDLK_BACKSPACE: return ImGuiKey.Backspace;
            case SDL_Keycode.SDLK_SPACE: return ImGuiKey.Space;
            case SDL_Keycode.SDLK_RETURN: return ImGuiKey.Enter;
            case SDL_Keycode.SDLK_ESCAPE: return ImGuiKey.Escape;
            case SDL_Keycode.SDLK_APOSTROPHE: return ImGuiKey.Apostrophe;
            case SDL_Keycode.SDLK_COMMA: return ImGuiKey.Comma;
            case SDL_Keycode.SDLK_MINUS: return ImGuiKey.Minus;
            case SDL_Keycode.SDLK_PERIOD: return ImGuiKey.Period;
            case SDL_Keycode.SDLK_SLASH: return ImGuiKey.Slash;
            case SDL_Keycode.SDLK_SEMICOLON: return ImGuiKey.Semicolon;
            case SDL_Keycode.SDLK_EQUALS: return ImGuiKey.Equal;
            case SDL_Keycode.SDLK_LEFTBRACKET: return ImGuiKey.LeftBracket;
            case SDL_Keycode.SDLK_BACKSLASH: return ImGuiKey.Backslash;
            case SDL_Keycode.SDLK_RIGHTBRACKET: return ImGuiKey.RightBracket;
            case SDL_Keycode.SDLK_GRAVE: return ImGuiKey.GraveAccent;
            case SDL_Keycode.SDLK_CAPSLOCK: return ImGuiKey.CapsLock;
            case SDL_Keycode.SDLK_SCROLLLOCK: return ImGuiKey.ScrollLock;
            case SDL_Keycode.SDLK_NUMLOCKCLEAR: return ImGuiKey.NumLock;
            case SDL_Keycode.SDLK_PRINTSCREEN: return ImGuiKey.PrintScreen;
            case SDL_Keycode.SDLK_PAUSE: return ImGuiKey.Pause;
            case SDL_Keycode.SDLK_LCTRL: return ImGuiKey.LeftCtrl;
            case SDL_Keycode.SDLK_LSHIFT: return ImGuiKey.LeftShift;
            case SDL_Keycode.SDLK_LALT: return ImGuiKey.LeftAlt;
            case SDL_Keycode.SDLK_LGUI: return ImGuiKey.LeftSuper;
            case SDL_Keycode.SDLK_RCTRL: return ImGuiKey.RightCtrl;
            case SDL_Keycode.SDLK_RSHIFT: return ImGuiKey.RightShift;
            case SDL_Keycode.SDLK_RALT: return ImGuiKey.RightAlt;
            case SDL_Keycode.SDLK_RGUI: return ImGuiKey.RightSuper;
            case SDL_Keycode.SDLK_APPLICATION: return ImGuiKey.Menu;
            case SDL_Keycode.SDLK_0: return ImGuiKey._0;
            case SDL_Keycode.SDLK_1: return ImGuiKey._1;
            case SDL_Keycode.SDLK_2: return ImGuiKey._2;
            case SDL_Keycode.SDLK_3: return ImGuiKey._3;
            case SDL_Keycode.SDLK_4: return ImGuiKey._4;
            case SDL_Keycode.SDLK_5: return ImGuiKey._5;
            case SDL_Keycode.SDLK_6: return ImGuiKey._6;
            case SDL_Keycode.SDLK_7: return ImGuiKey._7;
            case SDL_Keycode.SDLK_8: return ImGuiKey._8;
            case SDL_Keycode.SDLK_9: return ImGuiKey._9;
            case SDL_Keycode.SDLK_A: return ImGuiKey.A;
            case SDL_Keycode.SDLK_B: return ImGuiKey.B;
            case SDL_Keycode.SDLK_C: return ImGuiKey.C;
            case SDL_Keycode.SDLK_D: return ImGuiKey.D;
            case SDL_Keycode.SDLK_E: return ImGuiKey.E;
            case SDL_Keycode.SDLK_F: return ImGuiKey.F;
            case SDL_Keycode.SDLK_G: return ImGuiKey.G;
            case SDL_Keycode.SDLK_H: return ImGuiKey.H;
            case SDL_Keycode.SDLK_I: return ImGuiKey.I;
            case SDL_Keycode.SDLK_J: return ImGuiKey.J;
            case SDL_Keycode.SDLK_K: return ImGuiKey.K;
            case SDL_Keycode.SDLK_L: return ImGuiKey.L;
            case SDL_Keycode.SDLK_M: return ImGuiKey.M;
            case SDL_Keycode.SDLK_N: return ImGuiKey.N;
            case SDL_Keycode.SDLK_O: return ImGuiKey.O;
            case SDL_Keycode.SDLK_P: return ImGuiKey.P;
            case SDL_Keycode.SDLK_Q: return ImGuiKey.Q;
            case SDL_Keycode.SDLK_R: return ImGuiKey.R;
            case SDL_Keycode.SDLK_S: return ImGuiKey.S;
            case SDL_Keycode.SDLK_T: return ImGuiKey.T;
            case SDL_Keycode.SDLK_U: return ImGuiKey.U;
            case SDL_Keycode.SDLK_V: return ImGuiKey.V;
            case SDL_Keycode.SDLK_W: return ImGuiKey.W;
            case SDL_Keycode.SDLK_X: return ImGuiKey.X;
            case SDL_Keycode.SDLK_Y: return ImGuiKey.Y;
            case SDL_Keycode.SDLK_Z: return ImGuiKey.Z;
            case SDL_Keycode.SDLK_F1: return ImGuiKey.F1;
            case SDL_Keycode.SDLK_F2: return ImGuiKey.F2;
            case SDL_Keycode.SDLK_F3: return ImGuiKey.F3;
            case SDL_Keycode.SDLK_F4: return ImGuiKey.F4;
            case SDL_Keycode.SDLK_F5: return ImGuiKey.F5;
            case SDL_Keycode.SDLK_F6: return ImGuiKey.F6;
            case SDL_Keycode.SDLK_F7: return ImGuiKey.F7;
            case SDL_Keycode.SDLK_F8: return ImGuiKey.F8;
            case SDL_Keycode.SDLK_F9: return ImGuiKey.F9;
            case SDL_Keycode.SDLK_F10: return ImGuiKey.F10;
            case SDL_Keycode.SDLK_F11: return ImGuiKey.F11;
            case SDL_Keycode.SDLK_F12: return ImGuiKey.F12;
            case SDL_Keycode.SDLK_F13: return ImGuiKey.F13;
            case SDL_Keycode.SDLK_F14: return ImGuiKey.F14;
            case SDL_Keycode.SDLK_F15: return ImGuiKey.F15;
            case SDL_Keycode.SDLK_F16: return ImGuiKey.F16;
            case SDL_Keycode.SDLK_F17: return ImGuiKey.F17;
            case SDL_Keycode.SDLK_F18: return ImGuiKey.F18;
            case SDL_Keycode.SDLK_F19: return ImGuiKey.F19;
            case SDL_Keycode.SDLK_F20: return ImGuiKey.F20;
            case SDL_Keycode.SDLK_F21: return ImGuiKey.F21;
            case SDL_Keycode.SDLK_F22: return ImGuiKey.F22;
            case SDL_Keycode.SDLK_F23: return ImGuiKey.F23;
            case SDL_Keycode.SDLK_F24: return ImGuiKey.F24;
            case SDL_Keycode.SDLK_AC_BACK: return ImGuiKey.AppBack;
            case SDL_Keycode.SDLK_AC_FORWARD: return ImGuiKey.AppForward;
            default: break;
        }
        return ImGuiKey.None;
    }

    //protected void OnKeyDown(KeyEvent keyEvent)
    //{
    //    KeyPressed?.Invoke(keyEvent);
    //}

    private void HandleResize()
    {
        windowResized = true;
    }

    public void Close()
    {
        Running = false;
        //window.Close();
    }
}
