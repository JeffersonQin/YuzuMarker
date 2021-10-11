namespace YuzuMarker.Utils
{
    public static class ColorExtension
    {
        public static System.Windows.Media.Color ToColor(this uint color)
        {
            return System.Windows.Media.Color.FromArgb(
                (byte) ((color & 0xFF000000) >> 24),
                (byte) ((color & 0x00FF0000) >> 16),
                (byte) ((color & 0x0000FF00) >> 8),
                (byte) (color & 0x000000FF)
            );
        }
    }
}