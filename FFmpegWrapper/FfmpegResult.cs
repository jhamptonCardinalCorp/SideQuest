using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpegWrapper
{
    public sealed class FfmpegResult
    {
        public bool Success { get; init; }
        public string? OutputFile { get; init; }
        public string? Error { get; init; }
    }
}
