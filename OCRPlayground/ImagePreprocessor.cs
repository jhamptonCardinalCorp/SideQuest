
//  Static helper for preprocessing images. 

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Drawing;

namespace OCRPlayground
{
    public static class ImagePreprocessor
    {
        public static Bitmap Clean(Bitmap image)
        {
            // Convert Bitmap -> Mat
            Mat mat = BitmapExtension.ToMat(image);

            // Convert to grayscale
            CvInvoke.CvtColor(mat, mat, ColorConversion.Bgr2Gray);

            // Apply thresholding (makes text crisp)
            CvInvoke.Threshold(mat, mat, 0, 255, ThresholdType.Otsu | ThresholdType.Binary);

            // Optional denoise
            CvInvoke.GaussianBlur(mat, mat, new Size(3, 3), 0);

            // Convert back to Bitmap
            return BitmapExtension.ToBitmap(mat);
        }
    }
}
