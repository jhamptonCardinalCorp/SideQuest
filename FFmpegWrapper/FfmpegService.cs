using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpegWrapper
{
    public sealed class FfmpegService
    {
        private readonly string _ffmpegPath;

        public FfmpegService(string ffmpegPath)
        {
            _ffmpegPath = ffmpegPath;
        }

        public async Task<FfmpegResult> ExtractAudioAsync(
            string inputVideoPath,
            string outputAudioPath,
            CancellationToken cancellationToken = default)
        {
            var args = $"-i \"{inputVideoPath}\" -vn -acodec pcm_s16le \"{outputAudioPath}\"";

            var startInfo = new ProcessStartInfo
            {
                FileName = _ffmpegPath,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            try
            {
                using var process = new Process { StartInfo = startInfo, EnableRaisingEvents = true };

                process.Start();

                var stderrTask = process.StandardError.ReadToEndAsync(cancellationToken);
                var stdoutTask = process.StandardOutput.ReadToEndAsync(cancellationToken);

                await Task.WhenAll(stderrTask, stdoutTask);

                await process.WaitForExitAsync(cancellationToken);

                if (process.ExitCode == 0)
                {
                    return new FfmpegResult
                    {
                        Success = true,
                        OutputFile = outputAudioPath
                    };
                }

                return new FfmpegResult
                {
                    Success = false,
                    Error = stderrTask.Result
                };
            }
            catch (Exception ex)
            {
                return new FfmpegResult
                {
                    Success = false,
                    Error = ex.Message
                };
            }
        }
    }
}
