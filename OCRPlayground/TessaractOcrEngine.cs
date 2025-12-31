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
                //We switched EngineMode from Default to LstmOnly, as it's the modern neural model and has higher accuracy.
                using var engine = new TesseractEngine(_tessdataPath, _language, EngineMode.LstmOnly);
                // If we know the domain (logs, UI labels, code), this helps a ton:
                //engine.SetVariable("tessedit_char_whitelist", "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789:/.-_()[]{} ");
                using var pix = PixConverter.ToPix(image);
                //using var page = engine.Process(pix);
                //using var page = engine.Process(pix, PageSegMode.Auto);
                using var page = engine.Process(pix, PageSegMode.SingleBlock);

                return page.GetText();
            });
            // Other options for UI text include:
            // PageSegMode.SingleBlock
            // PageSegMode.SingleLine
            // PageSegMode.SingleWord
            //      TODO: add either a setting or a dynamic method for selection.
        }
    }
}
