using ArcadeFrontend.Interfaces;
using ArcadeFrontend.Providers;
using System;
using System.IO;
using Veldrid;
using Veldrid.SPIRV;

namespace ArcadeFrontend.Shaders
{
    public class ColorShader : Shader, IShader
    {
        public DeviceBuffer ProjectionBuffer { get; private set; }
        public DeviceBuffer ViewBuffer { get; private set; }
        public DeviceBuffer WorldBuffer { get; private set; }
        public CommandList CommandList { get; private set; }
        public Pipeline Pipeline { get; private set; }
        public ResourceSet ProjectionViewSet { get; private set; }
        public ResourceSet WorldTextureSet { get; private set; }

        public ColorShader(
            IApplicationWindow window,
            GraphicsDeviceProvider graphicsDeviceProvider)
            : base(window, graphicsDeviceProvider)
        {
        }

        public override void Load()
        {
            base.Load();

            var rf = ResourceFactory;

            ProjectionBuffer = rf.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
            ViewBuffer = rf.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
            WorldBuffer = rf.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));

            var shadersPath = Path.Combine(Environment.CurrentDirectory, @"Content/shader");
            var vertexShaderBytes = File.ReadAllBytes(Path.Combine(shadersPath, "WorldColor.vert.spv"));
            var fragmentShaderBytes = File.ReadAllBytes(Path.Combine(shadersPath, "WorldColor.frag.spv"));

            ShaderSetDescription shaderSet = new ShaderSetDescription(
                new[]
                {
                    new VertexLayoutDescription(
                        new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
                        new VertexElementDescription("Color", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4))
                },
                rf.CreateFromSpirv(
                    new ShaderDescription(ShaderStages.Vertex, vertexShaderBytes, "main"),
                    new ShaderDescription(ShaderStages.Fragment, fragmentShaderBytes, "main")));

            ResourceLayout projViewLayout = rf.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("ProjectionBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                    new ResourceLayoutElementDescription("ViewBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex)));

            ResourceLayout worldTextureLayout = rf.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("WorldBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex)));

            Pipeline = rf.CreateGraphicsPipeline(new GraphicsPipelineDescription(
                BlendStateDescription.SingleAlphaBlend,
                DepthStencilStateDescription.DepthOnlyLessEqual,
                RasterizerStateDescription.Default,
                PrimitiveTopology.TriangleList,
                shaderSet,
                new[] { projViewLayout, worldTextureLayout },
                MainSwapchain.Framebuffer.OutputDescription));

            ProjectionViewSet = rf.CreateResourceSet(new ResourceSetDescription(
                projViewLayout,
                ProjectionBuffer,
                ViewBuffer));

            WorldTextureSet = rf.CreateResourceSet(new ResourceSetDescription(
                worldTextureLayout,
                WorldBuffer));

            CommandList = rf.CreateCommandList();
        }

        public override void Unload()
        {
            base.Unload();

            CommandList.Dispose();
            CommandList = null;

            WorldTextureSet.Dispose();
            ProjectionViewSet.Dispose();
            Pipeline.Dispose();
            ProjectionBuffer.Dispose();
            ViewBuffer.Dispose();
            WorldBuffer.Dispose();
        }
    }
}
