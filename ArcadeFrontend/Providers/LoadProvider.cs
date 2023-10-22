using ArcadeFrontend.Interfaces;
using ArcadeFrontend.Render;
using ArcadeFrontend.Shaders;

namespace ArcadeFrontend.Providers
{
    public class LoadProvider : ILoadProvider
    {
        private readonly IApplicationWindow window;
        private readonly Sdl2WindowProvider sdl2WindowProvider;
        private readonly GraphicsDeviceProvider graphicsDeviceProvider;
        private readonly ImGuiProvider imGuiProvider;
        private readonly FileLoadProvider fileLoadProvider;
        //private readonly MapListProvider mapListProvider;
        //private readonly BlockRotationVerticesProvider blockRotationVerticesProvider;
        //private readonly ITextureResourcesLoadProvider textureResourcesProvider;
        //private readonly GameSpriteResourcesProvider spriteResourcesProvider;
        //private readonly ISkyboxResourcesLoadProvider skyboxResourcesProvider;
        //private readonly ISpriteVertexBufferLoadProvider spriteVertexBufferProvider;
        //private readonly ISkyboxVertexBufferLoadProvider skyboxVertexBufferProvider;
        //private readonly Camera camera;
        private readonly ColorShader colorShader;
        private readonly TextureShader textureShader;
        //private readonly SpriteShader spriteShader;
        //private readonly LineShader lineShader;
        //private readonly WindowFocusProvider windowFocusProvider;
        //private readonly IAppLoadProvider appLoadProvider;
        //private readonly DefaultInputsProvider defaultInputsProvider;
        //private readonly I18n i18n;
        //private readonly ActorTemplatesProvider actorTemplatesProvider;
        //private readonly PlayerTemplatesProvider playerTemplatesProvider;
        //private readonly ActorTraitDataProvider actorTraitDataProvider;
        //private readonly PlayerTraitDataProvider playerTraitDataProvider;
        //private readonly SpriteFrameIndexProvider spriteFrameIndexProvider;

        public LoadProvider(
            IApplicationWindow window,
            Sdl2WindowProvider sdl2WindowProvider,
            GraphicsDeviceProvider graphicsDeviceProvider,
            ImGuiProvider imGuiProvider,
            FileLoadProvider fileLoadProvider,
            //MapListProvider mapListProvider,
            //BlockRotationVerticesProvider blockRotationVerticesProvider,
            //ITextureResourcesLoadProvider textureResourcesProvider,
            //GameSpriteResourcesProvider spriteResourcesProvider,
            //ISkyboxResourcesLoadProvider skyboxResourcesProvider,
            //ISpriteVertexBufferLoadProvider spriteVertexBufferProvider,
            //ISkyboxVertexBufferLoadProvider skyboxVertexBufferProvider,
            //Camera camera,
            ColorShader colorShader,
            TextureShader textureShader
            //SpriteShader spriteShader,
            //LineShader lineShader,
            //WindowFocusProvider windowFocusProvider,
            //IAppLoadProvider appLoadProvider,
            //DefaultInputsProvider defaultInputsProvider,
            //I18n i18n,
            //ActorTraitDataProvider actorTraitDataProvider,
            //ActorTemplatesProvider actorTemplatesProvider,
            //SpriteFrameIndexProvider spriteFrameIndexProvider,
            //PlayerTemplatesProvider playerTemplatesProvider,
            //PlayerTraitDataProvider playerTraitDataProvider
            )
        {
            this.window = window;
            this.sdl2WindowProvider = sdl2WindowProvider;
            this.graphicsDeviceProvider = graphicsDeviceProvider;
            this.imGuiProvider = imGuiProvider;
            this.fileLoadProvider = fileLoadProvider;
            //this.mapListProvider = mapListProvider;
            //this.blockRotationVerticesProvider = blockRotationVerticesProvider;
            //this.textureResourcesProvider = textureResourcesProvider;
            //this.spriteResourcesProvider = spriteResourcesProvider;
            //this.skyboxResourcesProvider = skyboxResourcesProvider;
            //this.spriteVertexBufferProvider = spriteVertexBufferProvider;
            //this.skyboxVertexBufferProvider = skyboxVertexBufferProvider;
            //this.camera = camera;
            this.colorShader = colorShader;
            this.textureShader = textureShader;
            //this.spriteShader = spriteShader;
            //this.lineShader = lineShader;
            //this.windowFocusProvider = windowFocusProvider;
            //this.defaultInputsProvider = defaultInputsProvider;
            //this.appLoadProvider = appLoadProvider;
            //this.i18n = i18n;
            //this.actorTraitDataProvider = actorTraitDataProvider;
            //this.actorTemplatesProvider = actorTemplatesProvider;
            //this.spriteFrameIndexProvider = spriteFrameIndexProvider;
            //this.playerTemplatesProvider = playerTemplatesProvider;
            //this.playerTraitDataProvider = playerTraitDataProvider;
        }

        public void Load()
        {
            sdl2WindowProvider.Load();
            graphicsDeviceProvider.Load();
            window.Load();
            imGuiProvider.Load();
            fileLoadProvider.Load();
            //mapListProvider.Load();
            //i18n.Load();
            //actorTemplatesProvider.Load();
            //actorTraitDataProvider.Load();
            //playerTemplatesProvider.Load();
            //playerTraitDataProvider.Load();
            //spriteFrameIndexProvider.Load();
            //blockRotationVerticesProvider.Load();
            //textureResourcesProvider.Load();
            //spriteResourcesProvider.Load();
            //skyboxResourcesProvider.Load();
            //spriteVertexBufferProvider.Load();
            //skyboxVertexBufferProvider.Load();
            //camera.Load();

            colorShader.Load();
            //textureShader.Load();

            //spriteShader.Load();
            //lineShader.Load();
            //windowFocusProvider.Load();
            //defaultInputsProvider.Load();
            //appLoadProvider.Load(); // Always last
        }

        public void Unload()
        {
            //appLoadProvider.Unload(); // Always first
            //defaultInputsProvider.Unload();
            //windowFocusProvider.Unload();
            //lineShader.Unload();
            //spriteShader.Unload();

            //textureShader.Unload();
            colorShader.Unload();

            //camera.Unload();
            //skyboxVertexBufferProvider.Unload();
            //spriteVertexBufferProvider.Unload();
            //skyboxResourcesProvider.Unload();
            //spriteResourcesProvider.Unload();
            //textureResourcesProvider.Unload();
            //blockRotationVerticesProvider.Unload();
            //spriteFrameIndexProvider.Unload();
            //playerTraitDataProvider.Unload();
            //playerTemplatesProvider.Unload();
            //actorTraitDataProvider.Unload();
            //actorTemplatesProvider.Unload();
            //i18n.Unload();
            //mapListProvider.Unload();
            fileLoadProvider.Unload();
            imGuiProvider.Unload();
            window.Unload();
            graphicsDeviceProvider.Unload();
            sdl2WindowProvider.Unload();
        }
    }
}
