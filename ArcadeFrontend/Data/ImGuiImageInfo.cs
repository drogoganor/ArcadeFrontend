using System;
using System.Numerics;

namespace ArcadeFrontend.Data
{
    /// <summary>
    /// TextureView, Pointer, and IntPtr for an ImGui image binding.
    /// Used in "ImGuiImages" dictionaries
    /// </summary>
    public class ImGuiImageInfo
    {
        public nint Texture { get; set; }
        public IntPtr IntPtr { get; set; }
        public Vector2 PixelSize { get; set; }
    }
}
