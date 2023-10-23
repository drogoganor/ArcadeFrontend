using System;
using System.Numerics;
using Veldrid;

namespace ArcadeFrontend.Data
{
    /// <summary>
    /// TextureView, Pointer, and IntPtr for an ImGui image binding.
    /// Used in "ImGuiImages" dictionaries
    /// </summary>
    public class ImGuiImageInfo
    {
        public Texture Texture { get; set; }
        public TextureView TextureView { get; set; }
        public IntPtr IntPtr { get; set; }
        public Vector2 PixelSize { get; set; }
    }
}
