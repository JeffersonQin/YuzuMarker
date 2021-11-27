using System;
using OpenCvSharp;

namespace YuzuMarker.Utils
{
    public static partial class UMatExtension
    {
        public static bool IsEmpty(this UMat umat)
        {
            if (umat == null) return true;
            if (umat.IsDisposed) return true;
            if (umat.CvPtr != IntPtr.Zero)
                return Cv2.CountNonZero(umat) == 0;
            return true;
        }

        public static bool SafeDispose(this UMat umat)
        {
            if (umat == null) return false;
            if (umat.IsDisposed) return false;
            if (umat.CvPtr == IntPtr.Zero) return false;
            umat.Dispose();
            return true;
        }

        public static UMat SafeClone(this UMat umat)
        {
            if (umat == null) return null;
            if (umat.IsDisposed) return null;
            if (umat.CvPtr == IntPtr.Zero) return null;
            return umat.Clone();
        }
    }
}