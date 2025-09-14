using ArcadeFrontend.Enums;
using ArcadeFrontend.Menus;

namespace ArcadeFrontend.Providers;

public class AppMenuProvider : MenuProvider
{
    private readonly ArcadeUI arcadeUi;

    public AppMenuProvider(
        ArcadeUI arcadeUi)
    {
        this.arcadeUi = arcadeUi;
    }

    public override void Draw(float deltaSeconds)
    {
        if (IsVisible)
        {
            arcadeUi.Draw(deltaSeconds);
        }
    }

    public override void Show(MenuType menuType)
    {
        base.Show(menuType);

        Menu menu = menuType switch
        {
            MenuType.Main => arcadeUi,
            _ => arcadeUi
        };

        arcadeUi.Hide();

        menu.Show();
    }

    public override void Hide()
    {
        base.Hide();
    }
}
