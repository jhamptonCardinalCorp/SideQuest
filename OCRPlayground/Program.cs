// OCRPlayground
//      This is the experimental playground for using OCRs: Image to text
//      Currently, its using Tesseract and OpenCV(EmguCV) via nuget packages.
//  OpenCV is used to prep and clean up images to make it easier for Tesseract to 'read'.
//  Later, we'll see about adding in EasyOCR. However, it's purely Python based, so more steps will be needed.

// Workflow:
//      Image Capture => OpenCV Preprocessing Layer => IOcrEngine Interface => TesseractImpl||EasyOcrImpl => App logic

using System.Drawing;

namespace OCRPlayground
{
    class Program
    {
        // For now, we're hard coding Tessaract, since we haven't implimented EasyOCR.
        //      (It's going to be an event setting *that* up.)
        static PreprocessingOcrEngine ocrEngine = new PreprocessingOcrEngine(new TessaractOcrEngine());
        static bool isPathValid = false;
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, world.");

            string? imagePath = args.Count() > 0 ? args[0] : null;

            // If the user passed a path as an argument, try that first
            if (!String.IsNullOrWhiteSpace(imagePath) && File.Exists(imagePath))
            {
                isPathValid = true;
            }

            while (!isPathValid)
            {
                Console.WriteLine("Input image path:");
                imagePath = Console.ReadLine();

                if (!String.IsNullOrWhiteSpace(imagePath) && File.Exists(imagePath))
                {
                    isPathValid = true;
                }
                else Console.WriteLine("Invalid path. Try again.");
            }

            // Load the image into a Bitmap
            using Bitmap bmp = new Bitmap(imagePath);

            // Run OCR
            string extractedText = ocrEngine.ExtractTextAsync(bmp).Result;

            Console.WriteLine("Extracted text:");
            Console.WriteLine(extractedText);
            Console.WriteLine("Press any key to continue...");
            _ = Console.ReadKey();
        }
    }
}