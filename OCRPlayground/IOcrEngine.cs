using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCRPlayground
{
    public interface IOcrEngine
    {
        /// <summary>
        /// Extracts text from an image.
        /// </summary>
        /// <param name="image">The image to process.</param>
        /// <returns>The extracted text.</returns>
        Task<string> ExtractTextAsync(Bitmap image);
        // we can swap (or overload) Bitmap for Image, byte[], or a custom wrapper later,
        // but Bitmap is the most universal starting point in .NET.

        /// <summary>
        /// Optional: returns the name of the engine (e.g., "Tesseract", "EasyOCR").
        /// Useful for logging, debugging, or user selection.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Optional: allows engines to expose configuration or capabilites.
        /// </summary>
        OcrEngineCapabilites Capabilites { get; }
    }

}
