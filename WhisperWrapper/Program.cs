
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

public class Settings
{
    public Paths Paths { get; set; } = new();
    public Transcription Transcription { get; set; } = new();
    public RunConfig Run { get; set; } = new();
}

public class Paths
{
    public string PythonExe { get; set; } = "python";
    public string TranscribeScript { get; set; } = "transcribe.py";
    public string SpeakerScript { get; set; } = "speakers_placeholder.py";
    public string MediaRoot { get; set; } = "";
}

public class Transcription
{
    public string Model { get; set; } = "large-v3";
    public string Device { get; set; } = "cuda";
    public string ComputeType { get; set; } = "float16";
    public string? Language { get; set; } = null;
    public string Task { get; set; } = "transcribe";
}

public class RunConfig
{
    public bool WatchMode { get; set; } = false;
    public string InputFile { get; set; } = "";
    public bool StopOnError { get; set; } = true;
}

internal class Program
{
    private static async Task<int> Main(string[] args)
    {
        var settings = LoadSettings("appsettings.json");
        Log("Orchestrator starting...");

        if (settings.Run.WatchMode)
        {
            return await RunWatcherAsync(settings);
        }
        else
        {
            if (string.IsNullOrWhiteSpace(settings.Run.InputFile) || !File.Exists(settings.Run.InputFile))
            {
                Error($"Input file not found: {settings.Run.InputFile}");
                return 2;
            }
            return await ProcessOneAsync(settings, settings.Run.InputFile);
        }
    }

    static Settings LoadSettings(string path)
    {
        var json = File.ReadAllText(path);
        var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<Settings>(json, opts) ?? new Settings();
    }

    static async Task<int> RunWatcherAsync(Settings settings)
    {
        if (string.IsNullOrWhiteSpace(settings.Paths.MediaRoot) || !Directory.Exists(settings.Paths.MediaRoot))
        {
            Error($"MediaRoot not found: {settings.Paths.MediaRoot}");
            return 3;
        }

        Log($"Watching: {settings.Paths.MediaRoot}");
        using var fsw = new FileSystemWatcher(settings.Paths.MediaRoot)
        {
            Filter = "*.mp4",
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.CreationTime | NotifyFilters.Size
        };

        fsw.Created += async (s, e) =>
        {
            // small debounce for file copy completion
            await Task.Delay(TimeSpan.FromSeconds(2));
            await ProcessOneAsync(settings, e.FullPath);
        };
        fsw.EnableRaisingEvents = true;

        Log("Press Ctrl+C to stop.");
        await Task.Delay(Timeout.Infinite);
        return 0;
    }

    static async Task<int> ProcessOneAsync(Settings settings, string mediaPath)
    {
        try
        {
            Log($"[1/2] Transcribing: {mediaPath}");
            var transcribeExit = await RunPythonAsync(
                settings.Paths.PythonExe,
                settings.Paths.TranscribeScript,
                new[] { mediaPath },
                env: new Dictionary<string, string?> // you can pass extra env if needed
                {
                    // e.g., HF_TOKEN when diarization is ready
                }
            );

            if (transcribeExit != 0)
            {
                Error($"Transcription failed (exit {transcribeExit}).");
                return settings.Run.StopOnError ? transcribeExit : 0;
            }

            var basePath = Path.ChangeExtension(mediaPath, null); // removes .mp4
            var jsonPath = basePath + ".json";
            if (!File.Exists(jsonPath))
            {
                Error($"Expected JSON not found: {jsonPath}");
                return settings.Run.StopOnError ? 4 : 0;
            }

            Log($"[2/2] Speaker attribution (placeholder): {jsonPath}");
            var speakerExit = await RunPythonAsync(
                settings.Paths.PythonExe,
                settings.Paths.SpeakerScript,
                new[] { jsonPath },
                env: null
            );

            if (speakerExit != 0)
            {
                Error($"Speaker placeholder failed (exit {speakerExit}).");
                return settings.Run.StopOnError ? speakerExit : 0;
            }

            var speakerJson = basePath + ".speaker.json";
            if (!File.Exists(speakerJson))
            {
                Error($"Expected speaker JSON not found: {speakerJson}");
                return settings.Run.StopOnError ? 5 : 0;
            }

            Log("Success. Artifacts:");
            Log($"  TXT: {basePath}.txt");
            Log($"  SRT: {basePath}.srt");
            Log($"  JSON: {jsonPath}");
            Log($"  SPEAKER JSON: {speakerJson}");

            // TODO: hand off to summarizer here, if desired.

            return 0;
        }
        catch (Exception ex)
        {
            Error("Unhandled exception: " + ex);
            return 1;
        }
    }

    static async Task<int> RunPythonAsync(string pythonExe, string scriptPath, string[] args, Dictionary<string, string?>? env)
    {
        var psi = new ProcessStartInfo
        {
            FileName = pythonExe,
            Arguments = $"\"{scriptPath}\" {string.Join(' ', args.Select(a => $"\"{a}\""))}",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        if (env is not null)
        {
            foreach (var kvp in env)
            {
                psi.Environment[kvp.Key] = kvp.Value ?? "";
            }
        }

        using var proc = new Process { StartInfo = psi, EnableRaisingEvents = true };
        var tcs = new TaskCompletionSource<int>();

        proc.OutputDataReceived += (s, e) => { if (e.Data != null) Log("[py] " + e.Data); };
        proc.ErrorDataReceived += (s, e) => { if (e.Data != null) Error("[py] " + e.Data); };

        proc.Start();
        proc.BeginOutputReadLine();
        proc.BeginErrorReadLine();

        proc.Exited += (s, e) =>
        {
            tcs.TrySetResult(proc.ExitCode);
            proc.Dispose();
        };

        return await tcs.Task.ConfigureAwait(false);
    }

    static void Log(string msg) => Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {msg}");
    static void Error(string msg)
    {
        var prev = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {msg}");
        Console.ForegroundColor = prev;
    }
    static void GivePause()
    {
        Console.WriteLine("Press any key to continue..."); 
        Console.ReadKey();
    }
}
