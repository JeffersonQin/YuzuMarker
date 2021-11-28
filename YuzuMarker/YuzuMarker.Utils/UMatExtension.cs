using System;
using OpenCvSharp;

namespace YuzuMarker.Utils
{
    public static partial class UMatExtension
    {
        public static bool IsEmpty(this UMat src)
        {
            if (src == null) return true;
            if (src.IsDisposed) return true;
            if (src.CvPtr == IntPtr.Zero) return true;
            if (src.Channels() == 1)
                return Cv2.CountNonZero(src) == 0;
            UMat singleChannelMat = new UMat();
            Cv2.CvtColor(src, singleChannelMat, ColorConversionCodes.BGR2GRAY);
            bool isEmpty = Cv2.CountNonZero(singleChannelMat) == 0;
            singleChannelMat.SafeDispose();
            return isEmpty;
        }

        public static bool SafeDispose(this UMat src)
        {
            if (src == null) return false;
            if (src.IsDisposed) return false;
            if (src.CvPtr == IntPtr.Zero) return false;
            src.Dispose();
            return true;
        }

        public static UMat SafeClone(this UMat src)
        {
            if (src == null) return null;
            if (src.IsDisposed) return null;
            if (src.CvPtr == IntPtr.Zero) return null;
            return src.Clone();
        }
    }
}