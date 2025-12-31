
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace AutoMarionette
{
    public class AutoMarionette
    {
        [DllImport("User32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("User32.dll")]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("User32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("User32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        public const uint WM_KEYDOWN = 0x0100;
        public const uint WM_KEYUP = 0x0101;

        // Example: send the "A" key
        public static void SendKey(char key, string? windowTitle)
        {
            windowTitle = windowTitle ?? GetActiveWindowTitle();
            
            IntPtr hWnd = FindWindow(null, windowTitle); // Or use lpClassName if known
            if (hWnd != IntPtr.Zero)
            {
                PostMessage(hWnd, WM_KEYDOWN, (IntPtr)key, IntPtr.Zero);
                PostMessage(hWnd, WM_KEYUP, (IntPtr)key, IntPtr.Zero);
            }
        }

        public static void SendVirtualKey(int virtualKeyCode, string? windowTitle)
        {
            windowTitle = windowTitle ?? GetActiveWindowTitle();

            IntPtr hWnd = FindWindow(null, windowTitle);
            if (hWnd != IntPtr.Zero)
            {
                PostMessage(hWnd, WM_KEYDOWN, (IntPtr)virtualKeyCode, IntPtr.Zero);
                PostMessage(hWnd, WM_KEYUP, (IntPtr)virtualKeyCode, IntPtr.Zero);
            }
        }

        // Get active window
        public static string GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return string.Empty;
        }
    }
}