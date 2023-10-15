using ArcadeFrontend.Enums;
using ArcadeFrontend.Interfaces;
using ImGuiNET;
using System.Collections.Generic;
using Veldrid;

namespace ArcadeFrontend.Providers
{
    public class ImGuiProvider : ILoad
    {
        private readonly IApplicationWindow window;
        private readonly GraphicsDeviceProvider graphicsDeviceProvider;
        private readonly ModManifestProvider modManifestProvider;

        private CommandList commandList;
        private ImGuiRenderer imGuiRenderer;
        private readonly Dictionary<FontSize, ImFontPtr> fonts = new();

        public Dictionary<FontSize, ImFontPtr> Fonts => fonts;
        public CommandList CommandList => commandList;
        public ImGuiRenderer ImGuiRenderer => imGuiRenderer;

        public ImGuiProvider(
            IApplicationWindow window,
            GraphicsDeviceProvider graphicsDeviceProvider,
            ModManifestProvider modManifestProvider)
        {
            this.window = window;
            this.graphicsDeviceProvider = graphicsDeviceProvider;
            this.modManifestProvider = modManifestProvider;
        }

        public void Load()
        {
            var gd = graphicsDeviceProvider.GraphicsDevice;
            imGuiRenderer = new ImGuiRenderer(
                gd,
                gd.MainSwapchain.Framebuffer.OutputDescription,
                (int)window.Width,
                (int)window.Height);

            var isVulkan = gd.BackendType == GraphicsBackend.Vulkan;
            var io = ImGui.GetIO();

            foreach (var font in modManifestProvider.ModManifestFile.Fonts)
            {
                // Trouble with fonts in Vulkan.
                // https://github.com/veldrid/veldrid/issues/154#issuecomment-484372184
                if (isVulkan)
                {
                    fonts.Add(font.SizeEnum, io.Fonts.AddFontDefault());
                }
                else
                {
                    var imFontPtr = io.Fonts.AddFontFromFileTTF(@"C:\Windows\Fonts\" + font.FontName, font.FontSize);
                    fonts.Add(font.SizeEnum, imFontPtr);
                }
            }

            if (isVulkan)
            {
                //imGuiRenderer.RecreateFontDeviceTexture();
            }
            else
            {
                imGuiRenderer.RecreateFontDeviceTexture();
            }

            //ImGui.NewFrame();

            commandList = graphicsDeviceProvider.ResourceFactory.CreateCommandList();
        }

        public void Unload()
        {
            commandList.Dispose();
            commandList = null;

            ImGui.GetIO().Fonts.ClearFonts();
            fonts.Clear();

            imGuiRenderer.Dispose();
            imGuiRenderer = null;
        }
    }
}
