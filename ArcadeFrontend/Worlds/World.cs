using ArcadeFrontend.Interfaces;
using ArcadeFrontend.Providers;
using Veldrid;

namespace ArcadeFrontend.Worlds
{
    /// <summary>
    /// Tick and draw components
    /// </summary>
    public abstract class World : IWorld
    {
        private readonly ImGuiProvider imGuiProvider;
        private readonly ILoadProvider loadProvider;

        protected InputSnapshot InputSnapshot;

        public World(
            ImGuiProvider imGuiProvider,
            ILoadProvider loadProvider)
        {
            this.imGuiProvider = imGuiProvider;
            this.loadProvider = loadProvider;

            loadProvider.Load();
        }

        public abstract void Draw(float deltaSeconds);

        public virtual void Tick(float deltaSeconds)
        {
            InputSnapshot = InputTracker.FrameSnapshot;
            imGuiProvider.ImGuiRenderer.Update(1f / 60f, InputSnapshot);
        }
    }
}
