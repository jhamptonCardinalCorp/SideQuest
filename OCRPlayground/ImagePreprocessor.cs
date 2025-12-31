
//  Static helper for preprocessing images. 

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Reflection.Metadata;
using System.Runtime.ConstrainedExecution;
using static System.Net.Mime.MediaTypeNames;

namespace OCRPlayground
{
    public static class ImagePreprocessor
    {
        public static Bitmap Clean(Bitmap image)
        {
            // Convert Bitmap -> Mat
            Mat mat = BitmapExtension.ToMat(image);

            //// "Enhance" (Upscale)
            //CvInvoke.Resize(mat, mat, new Size(mat.Width * 2, mat.Height * 2), 0, 0, Inter.Linear);

            // Convert to grayscale
            CvInvoke.CvtColor(mat, mat, ColorConversion.Bgr2Gray);
            // Light contrast boost (optional)
            CvInvoke.Normalize(mat, mat, 0, 255, NormType.MinMax);

            //// Apply thresholding (makes text crisp) TODO: Modify to allow reactive change?
            ////CvInvoke.Threshold(mat, mat, 0, 255, ThresholdType.Otsu | ThresholdType.Binary);
            //// Otsu is global — good for consistent lighting, bad for gradients or anti‑aliased text.
            //// Adaptive thresholding handles UI text better:
            //CvInvoke.AdaptiveThreshold(
            //    mat, mat, 255,
            //    AdaptiveThresholdType.GaussianC,
            //    ThresholdType.Binary,
            //    11, 2
            //);

            //// Morphological opening (denoise)
            //var kernel = CvInvoke.GetStructuringElement(MorphShapes.Rectangle, new Size(1, 1), new Point(-1, -1));
            //CvInvoke.MorphologyEx(mat, mat, MorphOp.Open, kernel, new Point(-1, -1), 1, BorderType.Default, default);

            //// Light sharpening to make characters more distinct:
            //float[,] kernelBase =
            //{
            //    {-1, -1, -1},
            //    {-1, 9, -1 },
            //    {-1, -1, -1 }
            //};
            //var sharpenKernel = new ConvolutionKernelF(kernelBase);
            //CvInvoke.Filter2D(mat, mat, sharpenKernel, new Point(-1, -1));


            // Optional denoise; Better for photos apparently? Detrimental for UIs.
            //CvInvoke.GaussianBlur(mat, mat, new Size(3, 3), 0);

            // Straighten Text; Unbuilt
            //mat = Deskew(mat);

            // Convert back to Bitmap
            return BitmapExtension.ToBitmap(mat);
        }
    }
}
//We can build a dynamic preprocessor:
//• 	If the image has high contrast → skip thresholding
//• 	If the image is small → upscale
//• 	If the image has noise → denoise
//• 	If the image is code → use SingleLine
//• 	If the image is paragraph text → use Auto
//But let’s get you back to a stable baseline first

// Grayscal + Normalization seems to be the sweet spot for screen captures, at least when working with UIs.
// The rest would be more useful when working with scanned documents or (on occasion) photos.
// This does leave us with the question of where does EasyOCR fall. It's supposed to be good for "messy handwriting".