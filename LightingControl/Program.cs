// Name: LightingControl
// Desc: This is to help with some games being too dark, while also not blinding myself
// with auto-corrected or reactive brightness. Also, general learning of new systems and frameworks.

//This is in disrepair as of 30Dec2025. The Direct3D11Helper is referencing itself, and really, the whole things a mess.
// Now, the basic concept should still be sound, and the original files should still be usable as a base in the future:
// Program.cs, FrameData.cs, LuminanceStats.cs, ExposureEngine.cs, and GammaRamp.cs.

using System;
using Windows.Graphics.Capture;
using Windows.Graphics.DirectX;
using Windows.Graphics.DirectX.Direct3D11;

struct FrameData
{
    int width;
    int height;
    byte[] luminanceValues; // or RGB if you prefer
}

struct LuminanceStats
{
    float average;
    float median;
    float shadow10;
    float highlight90;
}

namespace LightingControl
{
    class Program
    {
        static void Main()
        {
            // Required for WinRT interop
            //WinRT.ComWrappersSupport.InitializeComWrappers();

            // Pick the primary monitor
            var monitor = CaptureHelper.GetPrimaryMonitorItem();

            // Create a frame pool
            var device = Direct3D11Helper.CreateDevice();
            var pool = Direct3D11CaptureFramePool.Create(
                device,
                DirectXPixelFormat.B8G8R8A8UIntNormalized,
                1,
                monitor.Size);

            var session = pool.CreateCaptureSession(monitor);

            pool.FrameArrived += (s, e) =>
            {
                using var frame = pool.TryGetNextFrame();
                Console.WriteLine($"Captured frame: {frame.ContentSize.Width}x{frame.ContentSize.Height}");
            };

            session.StartCapture();

            Console.WriteLine("Capturing... press Enter to exit.");
            Console.ReadLine();

        }
        //static bool running = false;

        //static async Task Main2()
        //{
        //    var monitor = CaptureHelper.GetPrimaryMonitorItem();

        //    while (running)
        //    {
        //        frame = capture.GetFrame();
        //        stats = analyzer.Compute(frame);
        //        decision = exposureEngine.Update(stats);
        //        outputBackend.Apply(decision);
        //        Sleep(100ms);
        //    }
        //}
    }
}