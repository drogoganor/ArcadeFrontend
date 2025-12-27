using System.Numerics;

namespace ArcadeFrontend;

public static class Utils
{
    public static Vector2 ScaleSizeProportionally(Vector2 imageSize, Vector2 maxSize)
    {
        var ratioX = (double)maxSize.X / imageSize.X;
        var ratioY = (double)maxSize.Y / imageSize.Y;
        var ratio = Math.Min(ratioX, ratioY);

        var newWidth = (int)(imageSize.X * ratio);
        var newHeight = (int)(imageSize.Y * ratio);

        return new Vector2(newWidth, newHeight);
    }
}
