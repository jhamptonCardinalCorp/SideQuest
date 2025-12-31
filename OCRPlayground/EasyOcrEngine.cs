using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCRPlayground
{
    public class EasyOcrEngine : IOcrEngine
    {
        public string Name => "EasyOCR";

        public OcrEngineCapabilites Capabilites => new()
        {
            SupportsGpu = true,
            SupportsLanguages = true,
            AvailableLanguages = ["en-US"]
        };

        public async Task<string> ExtractTextAsync(Bitmap image)
        {
            throw new NotImplementedException();
            // Likely calls a Python process or service
            //return await Task.Run(static () =>
            //{
            //    // EasyOCR logic here
            //});
        }
    }
}
