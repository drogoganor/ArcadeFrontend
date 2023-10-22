using ArcadeFrontend.Interfaces;
using ArcadeFrontend.Providers;
using ArcadeFrontend.Shaders;
using System.Numerics;
using Veldrid;

namespace ArcadeFrontend.Render
{
    public class WorldRenderer : IRenderable
    {
        private readonly GraphicsDeviceProvider graphicsDeviceProvider;
        private readonly ColorShader colorShader;
        private readonly TextureShader textureShader;
        private readonly FrontendSettingsProvider frontendSettingsProvider;
        private readonly Camera camera;

        public WorldRenderer(
            GraphicsDeviceProvider graphicsDeviceProvider,
            ColorShader colorShader,
            TextureShader textureShader,
            Camera camera,
            FrontendSettingsProvider frontendSettingsProvider)
        {
            this.graphicsDeviceProvider = graphicsDeviceProvider;
            this.colorShader = colorShader;
            this.textureShader = textureShader;
            this.camera = camera;
            this.frontendSettingsProvider = frontendSettingsProvider;

        }

        public void Draw(float deltaSeconds)
        {
            var settings = frontendSettingsProvider.Settings;
            var shader = colorShader;
            var cl = shader.CommandList;
            var gd = graphicsDeviceProvider.GraphicsDevice;

            cl.Begin();
            cl.UpdateBuffer(shader.ProjectionBuffer, 0, camera.ProjectionMatrix);
            cl.UpdateBuffer(shader.ViewBuffer, 0, camera.ViewMatrix);
            cl.UpdateBuffer(shader.WorldBuffer, 0, Matrix4x4.Identity);
            cl.SetFramebuffer(graphicsDeviceProvider.Framebuffer);
            cl.ClearColorTarget(0, new RgbaFloat(settings.BackgroundColor));
            cl.ClearDepthStencil(1f);
            cl.SetPipeline(shader.Pipeline);

            //if (mapVertexBufferProvider.VertexBuffer != null)
            //{
            //    cl.SetVertexBuffer(0, mapVertexBufferProvider.VertexBuffer);
            //    cl.SetGraphicsResourceSet(0, shader.ProjectionViewSet);
            //    cl.SetGraphicsResourceSet(1, shader.WorldTextureSet);
            //    cl.Draw((uint)mapVertexBufferProvider.Vertices.Length);
            //}

            cl.End();
            gd.SubmitCommands(cl);
        }
    }
}
