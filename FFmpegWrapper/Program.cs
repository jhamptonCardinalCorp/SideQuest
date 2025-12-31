


namespace FFmpegWrapper
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var ffmpeg = new FfmpegService(@"C:\Users\jhampton\OneDrive - Cardinal Glass Industries\Desktop\ffmpeg\ffmpeg.exe");

            var result = await ffmpeg.ExtractAudioAsync(
                @"C:\Users\jhampton\Downloads\sample-5s.mp4",
                @"C:\Users\jhampton\Downloads\output2.wav");

            if (result.Success)
            {
                Console.WriteLine($"Audio saved to: {result.OutputFile}");
            }
            else
            {
                Console.WriteLine($"FFmpeg error: {result.Error}");
            }
        }
    }
}