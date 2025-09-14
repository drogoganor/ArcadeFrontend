using ArcadeFrontend.Enums;

namespace ArcadeFrontend.Interfaces;

public interface IMenuProvider : IRenderable
{
    bool IsVisible { get; }
    MenuType MenuType { get; }
    void Show(MenuType menuType);
    void Hide();
}
