using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Barony
{
    internal class HotKeyControl
    {
        // Signaling primitives
        private static CancellationTokenSource _cts = new CancellationTokenSource();
        private static ManualResetEventSlim _runEvent = new ManualResetEventSlim(false);

        static void Main()
        {
            Console.WriteLine("Controls: Ctrl+T = Toggle Start/Pause | Ctrl+Q = Quit\n");

            // Launch worker on a background task
            Task.Factory.StartNew(TaskLoop, _cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            // Main thread only listens for key strokes
            while (!_cts.IsCancellationRequested)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    // Toggle start/pause
                    if (key.Key == ConsoleKey.T && key.Modifiers.HasFlag(ConsoleModifiers.Control))
                    {
                        if (_runEvent.IsSet)
                        {
                            _runEvent.Reset();
                            Console.WriteLine("▶️ Paused");
                        }
                        else
                        {
                            _runEvent.Set();
                            Console.WriteLine("▶️ Running");
                        }
                    }
                    // Quit the app
                    else if (key.Key == ConsoleKey.Q && key.Modifiers.HasFlag(ConsoleModifiers.Control))
                    {
                        Console.WriteLine("🚪 Exiting...");
                        _cts.Cancel();
                        // Make sure worker isn’t stuck waiting
                        _runEvent.Set();
                    }
                }

                Thread.Sleep(50);
            }
        }

        static void TaskLoop()
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                // Wait until Ctrl+T sets the event
                _runEvent.Wait(_cts.Token);

                // Your repeating work goes here:
                DoOneIteration();

                // Optional delay so you don’t hammer the CPU
                Thread.Sleep(200);
            }
        }

        static void DoOneIteration()
        {
            // Replace with your button-press or task logic
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Task iteration");
        }
    }
}