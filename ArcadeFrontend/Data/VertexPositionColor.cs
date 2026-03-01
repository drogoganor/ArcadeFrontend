using System.Numerics;
using Vortice.Mathematics;

namespace ArcadeFrontend.Data;

public struct VertexPositionColor
{
    public const uint SizeInBytes = 28;

    public float PosX;
    public float PosY;
    public float PosZ;

    public float ColorR;
    public float ColorG;
    public float ColorB;
    public float ColorA;

    public VertexPositionColor(Vector3 pos, Color4 color)
    {
        PosX = pos.X;
        PosY = pos.Y;
        PosZ = pos.Z;
        ColorR = color.R;
        ColorG = color.G;
        ColorB = color.B;
        ColorA = color.A;
    }
}
