using System;
using OpenCvSharp;

namespace YuzuMarker.Utils
{
    public static class MatExtension
    {
        public static bool IsEmpty(this Mat src)
        {
            if (src == null) return true;
            if (src.IsDisposed) return true;
            if (src.CvPtr == IntPtr.Zero) return true;
            if (src.Channels() == 1)
                return Cv2.CountNonZero(src) == 0;
            Mat singleChannelMat = new Mat();
            Cv2.CvtColor(src, singleChannelMat, ColorConversionCodes.BGR2GRAY);
            bool isEmpty = Cv2.CountNonZero(singleChannelMat) == 0;
            singleChannelMat.SafeDispose();
            return isEmpty;
        }

        public static bool SafeDispose(this Mat src)
        {
            if (src == null) return false;
            if (src.IsDisposed) return false;
            if (src.CvPtr == IntPtr.Zero) return false;
            src.Dispose();
            return true;
        }

        public static Mat SafeClone(this Mat src)
        {
            if (src == null) return null;
            if (src.IsDisposed) return null;
            if (src.CvPtr == IntPtr.Zero) return null;
            return src.Clone();
        }
    }
}