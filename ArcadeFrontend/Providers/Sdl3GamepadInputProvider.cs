//using ArcadeFrontend.Enums;
//using ArcadeFrontend.Interfaces;
//using Microsoft.Extensions.Logging;
//using static SDL3.SDL;

//namespace ArcadeFrontend.Providers;

//public class Sdl3GamepadInputProvider : ITick
//{
//    private readonly ILogger<Sdl3GamepadInputProvider> logger;

//    private nint gamepad; // Just one for now

//    public Sdl3GamepadInputProvider(
//        ILogger<Sdl3GamepadInputProvider> logger)
//    {
//        this.logger = logger;
//    }

//    public void GamepadAdded(nint gamepad)
//    {
//        this.gamepad = gamepad;
//    }

//    public void GamepadRemoved(nint gamepad)
//    {
//        this.gamepad = nint.Zero;
//    }

//    public void Tick(float deltaSeconds)
//    {
//    }
//}
