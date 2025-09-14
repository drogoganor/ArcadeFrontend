using ArcadeFrontend.Interfaces;
using ArcadeFrontend.Providers;
using System.Numerics;

namespace ArcadeFrontend.Render;

public class Camera : ILoad
{
    public Matrix4x4 ViewMatrix { get; private set; }
    public Matrix4x4 ProjectionMatrix { get; private set; }

    public Vector3 Position { get; set; } = new Vector3(0, 0, 0);
    public Vector3 Direction { get; private set; } = new Vector3(0, 0, 1f);

    public Vector3 Up { get; private set; } = Vector3.UnitY;

    public float Yaw { get; set; }
    public float Pitch { get; set; }

    public Quaternion Orientation { get; set; } = Quaternion.Identity;


    private readonly float fov = 1f;
    private readonly float near = 0.005f;
    private readonly float far = 1000f;

    private float windowWidth;
    private float windowHeight;

    private readonly FrontendSettingsProvider frontendSettingsProvider;

    public Camera(
        FrontendSettingsProvider frontendSettingsProvider)
    {
        this.frontendSettingsProvider = frontendSettingsProvider;
    }

    public void Load()
    {
        Yaw = 0;
        Pitch = 0;
        Position = Vector3.Zero;
        Direction = Vector3.UnitZ;

        var settings = frontendSettingsProvider.Settings.Video;
        windowWidth = settings.CurrentSize.X;
        windowHeight = settings.CurrentSize.Y;
        UpdateProjectionMatrix();
        UpdateDirectionAndView();
    }

    public void Unload()
    {
    }

    public void Move(Vector3 motionDir, float distance)
    {
        if (motionDir != Vector3.Zero)
        {
            var moveDirection = Vector3.Transform(
                Vector3.Transform(
                    motionDir,
                    Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, Pitch)),
                Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, -Yaw));

            Position += moveDirection * distance;
            UpdateDirectionAndView();
        }
    }

    public void ChangePitchYaw(Vector2 offset)
    {
        Yaw += offset.X;
        Pitch += offset.Y;
        Pitch = Clamp(Pitch, -1.55f, 1.55f);

        UpdateDirectionAndView();
    }

    private float Clamp(float value, float min, float max)
    {
        return value > max
            ? max
            : value < min
                ? min
                : value;
    }

    public void WindowResized(float width, float height)
    {
        windowWidth = width;
        windowHeight = height;
        UpdateProjectionMatrix();
    }

    private void UpdateProjectionMatrix()
    {
        var settings = frontendSettingsProvider.Settings.Video;
        windowWidth = settings.CurrentSize.X;
        windowHeight = settings.CurrentSize.Y;

        ProjectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(fov, windowWidth / windowHeight, near, far);
    }

    public void UpdateDirectionAndView()
    {
        Orientation = Quaternion.CreateFromRotationMatrix(
            Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, Pitch) * Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, -Yaw));

        Direction = Vector3.Transform(Vector3.UnitZ, Orientation);
        Direction = Vector3.Normalize(Direction);
        UpdateViewMatrix();
    }

    public void UpdateViewMatrix()
    {
        ViewMatrix = Matrix4x4.CreateLookAt(Position, Position + Direction, Vector3.UnitY);
    }
}
