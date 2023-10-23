using ArcadeFrontend.Data;

namespace ArcadeFrontend.Providers
{
    public class FrontendStateProvider
    {
        private readonly FrontendState state = new();
        public FrontendState State => state;

        public FrontendStateProvider()
        {
        }
    }
}
