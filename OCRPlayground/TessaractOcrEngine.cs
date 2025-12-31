using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesseract;

namespace OCRPlayground
{
    public class TessaractOcrEngine : IOcrEngine
    {
        public string Name => "Tesseract";

        public OcrEngineCapabilites Capabilites => new()
        {
            SupportsGpu = false,
            SupportsLanguages = true,
            AvailableLanguages = ["eng"] // TODO: Load dynamically later
        };

        private readonly string _tessdataPath;
        private readonly string _language;

        public TessaractOcrEngine(string tessdataPath = "tessdata", string language = "eng")
        {
            _tessdataPath = tessdataPath;
            _language = language;
        }
        public async Task<string> ExtractTextAsync(Bitmap image)
        {
            return await Task.Run(() =>
            {
                using var engine = new TesseractEngine(_tessdataPath, _language, EngineMode.Default);
                using var pix = PixConverter.ToPix(image);
                using var page = engine.Process(pix);

                return page.GetText();
            });
        }
    }
}
