using ArcadeFrontend.Interfaces;
using ArcadeFrontend.Providers;

namespace ArcadeFrontend.Worlds
{
    /// <summary>
    /// Tick and draw game components
    /// </summary>
    public class ArcadeWorld : World
    {
        private readonly IMenuProvider menuProvider;
        //private readonly OrthoSpriteRenderer orthoSpriteRenderer;
        //private readonly WorldRenderer worldRenderer;

        public ArcadeWorld(
            ImGuiProvider imGuiProvider,
            ILoadProvider loadProvider,
            IMenuProvider menuProvider
            //GameSpriteRenderer gameSpriteRenderer,
            //OrthoSpriteRenderer orthoSpriteRenderer
            )
            : base(imGuiProvider, loadProvider)
        {
            this.menuProvider = menuProvider;
            //this.gameSpriteRenderer = gameSpriteRenderer;
            //this.orthoSpriteRenderer = orthoSpriteRenderer;
        }

        public override void Draw(float deltaSeconds)
        {
            menuProvider.Draw(deltaSeconds);
        }

        public override void Tick(float deltaSeconds)
        {
            base.Tick(deltaSeconds);
        }
    }
}
