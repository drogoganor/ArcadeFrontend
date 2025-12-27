using System.Numerics;
using System.Runtime.InteropServices;

namespace ArcadeFrontend.Platform;

/// <summary>
/// Get the screen resolution
/// Unfortunately we don't have a platform independent way to do this right now, and we don't want to use System.Windows.Forms
/// So we're using user32.dll
/// https://stackoverflow.com/questions/53831644/how-to-get-the-screen-resolution-without-windows-forms-reference#:~:text=If%20you%20don't%20want,that%20is%20also%20available%20on%20.
/// </summary>
public static partial class MonitorResolution
{
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);

    [StructLayout(LayoutKind.Sequential)]
    public struct DEVMODE
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
        public string dmDeviceName;
        public short dmSpecVersion;
        public short dmDriverVersion;
        public short dmSize;
        public short dmDriverExtra;
        public int dmFields;
        public int dmPositionX;
        public int dmPositionY;
        public int dmDisplayOrientation;
        public int dmDisplayFixedOutput;
        public short dmColor;
        public short dmDuplex;
        public short dmYResolution;
        public short dmTTOption;
        public short dmCollate;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
        public string dmFormName;
        public short dmLogPixels;
        public int dmBitsPerPel;
        public int dmPelsWidth;
        public int dmPelsHeight;
        public int dmDisplayFlags;
        public int dmDisplayFrequency;
        public int dmICMMethod;
        public int dmICMIntent;
        public int dmMediaType;
        public int dmDitherType;
        public int dmReserved1;
        public int dmReserved2;
        public int dmPanningWidth;
        public int dmPanningHeight;
    }

    public static Vector2 GetDisplayResolution()
    {
        const int ENUM_CURRENT_SETTINGS = -1;

        DEVMODE devMode = default;
        devMode.dmSize = (short)Marshal.SizeOf(devMode);
        EnumDisplaySettings(null, ENUM_CURRENT_SETTINGS, ref devMode);

        return new Vector2(devMode.dmPelsWidth, devMode.dmPelsHeight);
    }
}
