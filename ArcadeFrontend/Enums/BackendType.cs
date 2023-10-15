namespace ArcadeFrontend.Enums
{
    /// <summary>
    /// Approved backend types.
    /// Corresponds to Veldrid.GraphicsBackendType
    /// </summary>
    public enum BackendType : byte
    {
        Direct3D11,
        Vulkan,
        OpenGL,
        //Metal,
        //OpenGLES,
    }
}
