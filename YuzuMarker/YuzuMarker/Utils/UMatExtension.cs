using System;
using System.Windows.Media.Imaging;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;

namespace YuzuMarker.Utils
{
    public static partial class UMatExtension
    {
        public static WriteableBitmap To4ChannelImage(this UMat umat, Scalar color)
        {
            if (umat.IsEmpty()) return null;
            // Create 4 channel output image with color on
            var backgroundMat = new Mat(new Size(umat.Cols, umat.Rows), MatType.CV_8UC4, color);
            // Create mask array, used for merging
            var maskArray = new Mat[4];
            // Get Mat of 1 Channel mask
            var c1Mask = umat.GetMat(AccessFlag.READ);
            // Assign mask array
            for (var i = 0; i < 4; i ++)
                maskArray[i] = c1Mask;
            // Create 4 channel mask instance
            var dstMask = new Mat();
            // Merge mask array into one single mask
            Cv2.Merge(maskArray, dstMask);
            // Create render mat instance
            var outputMat = new Mat();
            // Perform bitwise and to generate rendering mat
            Cv2.BitwiseAnd(backgroundMat, dstMask, outputMat);
            // Convert rendering mat to wpf image source
            var image = outputMat.ToWriteableBitmap();
            // Dispose mat objects
            outputMat.Dispose();
            dstMask.Dispose();
            c1Mask.Dispose();
            return image;
        }
    }
}