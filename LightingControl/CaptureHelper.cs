using System.Linq;
using System.Runtime.InteropServices;
using Windows.Graphics;
using Windows.Graphics.Capture;
using Windows.Graphics.DirectX.Direct3D11;

public static class CaptureHelper1
{
    public static GraphicsCaptureItem GetPrimaryMonitorItem()
    {
        var interop = Windows.Graphics.Capture.GraphicsCaptureItemInterop.CreateForMonitor(
            MonitorHelper.GetPrimaryMonitorHandle());
        return interop;
    }
}


public static class CaptureHelper
{
    public static GraphicsCaptureItem GetPrimaryMonitorItem()
    {
        var hMonitor = MonitorFromPoint(new POINT { X = 0, Y = 0 }, 2);
        return GraphicsCaptureItem.CreateFromMonitor(hMonitor);
    }

    [DllImport("user32.dll")]
    private static extern IntPtr MonitorFromPoint(POINT pt, uint flags);

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int X;
        public int Y;
    }
}