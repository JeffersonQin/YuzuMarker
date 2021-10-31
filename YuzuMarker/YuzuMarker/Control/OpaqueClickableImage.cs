using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace YuzuMarker.Control
{
    /// <summary>
    /// OpaqueClickableImage<br/>
    /// Modified from: https://stackoverflow.com/questions/4800597/wpf-detect-image-click-only-on-non-transparent-portion
    /// </summary>
    public class OpaqueClickableImage : Image
    {
        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            var source = (BitmapSource)Source;

            // Get the pixel of the source that was hit
            var x = (int)(hitTestParameters.HitPoint.X / ActualWidth * source.PixelWidth);
            var y = (int)(hitTestParameters.HitPoint.Y / ActualHeight * source.PixelHeight);

            if (x >= source.PixelWidth) x = source.PixelWidth - 1;
            if (y >= source.PixelHeight) y = source.PixelHeight - 1;
            
            // Copy the single pixel into a new byte array representing RGBA
            var pixel = new byte[4];
            source.CopyPixels(new Int32Rect(x, y, 1, 1), pixel, 4, 0);

            // Check the alpha (transparency) of the pixel
            // - threshold can be adjusted from 0 to 255
            if (pixel[3] < 1)
                return null;

            return new PointHitTestResult(this, hitTestParameters.HitPoint);
        }
    }
}