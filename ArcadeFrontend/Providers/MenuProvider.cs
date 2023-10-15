using ArcadeFrontend.Enums;
using ArcadeFrontend.Interfaces;

namespace ArcadeFrontend.Providers
{
    public abstract class MenuProvider : IMenuProvider
    {
        public bool IsVisible { get; private set; }

        public MenuType MenuType { get; protected set; }

        public virtual void Show(MenuType menuType)
        {
            MenuType = menuType;
            IsVisible = true;
        }

        public virtual void Hide()
        {
            IsVisible = false;
        }

        public abstract void Draw(float deltaSeconds);
    }
}
