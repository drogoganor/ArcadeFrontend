namespace ArcadeFrontend.Interfaces;

public interface IWorld : ITick, IRenderable
{
    void DrawUI(float deltaSeconds);
    void PostDraw(float deltaSeconds);
}
