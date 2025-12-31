using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace OCRSimpleUI
{
    public static class MonitorSelector
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MONITORINFO
        {
            public int cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public uint dwFlags;
        }

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromRect(ref RECT lprc, uint dwFlags);

        [DllImport("user32.dll")]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        private const uint MONITOR_DEFAULTTONEAREST = 2;

        public static MONITORINFO GetMonitorForWindow(System.Windows.Window window)
        {
            var hwnd = new System.Windows.Interop.WindowInteropHelper(window).Handle;

            if (hwnd == IntPtr.Zero)
                throw new InvalidOperationException("Window handle not created yet.");

            GetWindowRect(hwnd, out RECT rect);

            IntPtr hMonitor = MonitorFromRect(ref rect, MONITOR_DEFAULTTONEAREST);

            var mi = new MONITORINFO();
            mi.cbSize = Marshal.SizeOf(typeof(MONITORINFO));
            GetMonitorInfo(hMonitor, ref mi);

            return mi;
        }
    }
}