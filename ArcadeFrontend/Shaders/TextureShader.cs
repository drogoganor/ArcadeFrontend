using ArcadeFrontend.Enums;
using ArcadeFrontend.Interfaces;
using ArcadeFrontend.Providers;
using Veldrid;
using Veldrid.SPIRV;

namespace ArcadeFrontend.Shaders;

public class TextureShader : Shader, ILoad, IShader
{
    public DeviceBuffer ProjectionBuffer { get; private set; }
    public DeviceBuffer ViewBuffer { get; private set; }
    public DeviceBuffer WorldBuffer { get; private set; }
    public CommandList CommandList { get; private set; }
    public Pipeline Pipeline { get; private set; }
    public ResourceSet ProjectionViewSet { get; private set; }
    public ResourceSet WorldTextureSet { get; private set; }
    public TextureView SurfaceTextureView { get; private set; }

    private readonly IGraphicsDeviceProvider graphicsDeviceProvider;
    //private readonly ITextureResourcesProvider textureResourcesProvider;
    private readonly FrontendSettingsProvider frontendSettingsProvider;

    public TextureShader(IApplicationWindow window,
        IGraphicsDeviceProvider graphicsDeviceProvider,
        //ITextureResourcesProvider textureResourcesProvider,
        FrontendSettingsProvider frontendSettingsProvider)
        : base(window, graphicsDeviceProvider)
    {
        this.graphicsDeviceProvider = graphicsDeviceProvider;
        //this.textureResourcesProvider = textureResourcesProvider;
        this.frontendSettingsProvider = frontendSettingsProvider;
    }

    public override void Load()
    {
        base.Load();

        var rf = graphicsDeviceProvider.ResourceFactory;
        var settings = frontendSettingsProvider.Settings.Video;
        var sampler = settings.SpriteSamplerType switch
        {
            SamplerType.Point => GraphicsDevice.PointSampler,
            SamplerType.Linear => GraphicsDevice.LinearSampler,
            SamplerType.Aniso4x => GraphicsDevice.Aniso4xSampler,
            _ => GraphicsDevice.PointSampler
        };

        ProjectionBuffer = rf.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
        ViewBuffer = rf.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
        WorldBuffer = rf.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));

        //SurfaceTextureView = textureResourcesProvider.TextureView;

        var shadersPath = Path.Combine(Environment.CurrentDirectory, @"Content/shader");
        var vertexShaderBytes = File.ReadAllBytes(Path.Combine(shadersPath, "WorldTexture.vert.spv"));
        var fragmentShaderBytes = File.ReadAllBytes(Path.Combine(shadersPath, "WorldTexture.frag.spv"));

        ShaderSetDescription shaderSet = new ShaderSetDescription(
            new[]
            {
                new VertexLayoutDescription(
                    new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
                    new VertexElementDescription("TexCoords", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3))
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
                new ResourceLayoutElementDescription("WorldBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                new ResourceLayoutElementDescription("SurfaceTexture", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                new ResourceLayoutElementDescription("SurfaceSampler", ResourceKind.Sampler, ShaderStages.Fragment)));

        Pipeline = rf.CreateGraphicsPipeline(new GraphicsPipelineDescription(
            BlendStateDescription.SingleOverrideBlend,
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
            WorldBuffer,
            SurfaceTextureView,
            sampler));

        CommandList = rf.CreateCommandList();
    }

    public override void Unload()
    {
        base.Unload();

        if (CommandList != null)
        {
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
