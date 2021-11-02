using System;
using System.Globalization;
using System.Windows.Data;
using OpenCvSharp;
using YuzuMarker.Properties;
using YuzuMarker.Utils;

namespace YuzuMarker.Converter
{
    public class MaskUMat2Bitmap : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var umat = (UMat)value;
            return umat.To4ChannelImage(Settings.Default.SelectionMaskFillColor.ToScalar());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}