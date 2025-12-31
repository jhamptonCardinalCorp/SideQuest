
using Emgu.CV.OCR;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCRPlayground
{
    public class PreprocessingOcrEngine : IOcrEngine
    {
        private readonly IOcrEngine _inner;

        public PreprocessingOcrEngine(IOcrEngine inner)
        {
            _inner = inner;
        }

        public string Name => $"{_inner.Name}" + "Preprocessing";

        public OcrEngineCapabilites Capabilites => _inner.Capabilites;

        public async Task<string> ExtractTextAsync(Bitmap image)
        {
            var cleaned = ImagePreprocessor.Clean(image); // OpenCV/EmguCV
            return await _inner.ExtractTextAsync(cleaned);
        }
    }
}
