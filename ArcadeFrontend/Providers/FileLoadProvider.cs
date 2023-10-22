using ArcadeFrontend.Interfaces;

namespace ArcadeFrontend.Providers
{
    /// <summary>
    /// Loads actual game data from the mod manifest.
    /// Required so that block and sprite editors do not automatically load the default block info.
    /// </summary>
    public class FileLoadProvider : ILoad
    {
        private readonly IFileSystem fileSystem;
        private readonly ManifestProvider manifestProvider;
        //private readonly IBlocksFileLoadProvider blocksFileProvider;
        //private readonly TextureFileProvider textureFileProvider;
        //private readonly ISpriteSheetFileLoadProvider spriteSheetFileProvider;
        //private readonly SkyboxFileProvider skyboxFileProvider;
        //private readonly SoundFileProvider soundFileProvider;
        //private readonly IInputFileLoadProvider inputFileProvider;
        //private readonly DecalsFileProvider decalsFileProvider;
        //private readonly LanguagesFileProvider languagesFileProvider;
        //private readonly NotificationsFileProvider notificationsFileProvider;

        public FileLoadProvider(
            IFileSystem fileSystem,
            ManifestProvider manifestProvider
            //IBlocksFileLoadProvider blocksFileProvider,
            //TextureFileProvider textureFileProvider,
            //ISpriteSheetFileLoadProvider spriteSheetFileProvider,
            //SkyboxFileProvider skyboxFileProvider,
            //SoundFileProvider soundFileProvider,
            //IInputFileLoadProvider inputFileProvider,
            //DecalsFileProvider decalsFileProvider,
            //LanguagesFileProvider languagesFileProvider,
            //NotificationsFileProvider notificationsFileProvider
            )
        {
            this.fileSystem = fileSystem;
            this.manifestProvider = manifestProvider;
            //this.blocksFileProvider = blocksFileProvider;
            //this.textureFileProvider = textureFileProvider;
            //this.spriteSheetFileProvider = spriteSheetFileProvider;
            //this.skyboxFileProvider = skyboxFileProvider;
            //this.soundFileProvider = soundFileProvider;
            //this.inputFileProvider = inputFileProvider;
            //this.decalsFileProvider = decalsFileProvider;
            //this.languagesFileProvider = languagesFileProvider;
            //this.notificationsFileProvider = notificationsFileProvider;
        }

        public void Load()
        {
            //textureFileProvider.Load();
            //spriteSheetFileProvider.Load();
            //skyboxFileProvider.Load();
            //soundFileProvider.Load();
            //decalsFileProvider.Load();
            //blocksFileProvider.Load();
            //inputFileProvider.Load();
            //languagesFileProvider.Load();
            //notificationsFileProvider.Load();
        }

        public void Unload()
        {
            //notificationsFileProvider.Unload();
            //languagesFileProvider.Unload();
            //inputFileProvider.Unload();
            //blocksFileProvider.Unload();
            //decalsFileProvider.Unload();
            //soundFileProvider.Unload();
            //skyboxFileProvider.Unload();
            //spriteSheetFileProvider.Unload();
            //textureFileProvider.Unload();
        }
    }
}
