using System.Drawing;
using OpenCvSharp;

namespace YuzuMarker.Utils
{
    public static class ScalarExtension
    {
        public static Scalar ToScalar(this uint color)
        {
            return new Scalar(
                color & 0x000000FF,
                (color & 0x0000FF00) >> 8,
                (color & 0x00FF0000) >> 16,
                (color & 0xFF000000) >> 24);
        }

        public static Scalar ToScalar(this Color color)
        {
            return new Scalar(color.B, color.G, color.R, color.A);
        }
    }
}