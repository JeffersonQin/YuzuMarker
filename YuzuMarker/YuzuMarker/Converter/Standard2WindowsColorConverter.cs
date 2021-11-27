using System;
using System.Globalization;
using System.Windows.Data;

namespace YuzuMarker.Converter
{
    public class Standard2WindowsColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            if (value.GetType() != typeof(System.Drawing.Color)) return null;
            var color = (System.Drawing.Color)value;
            return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            if (value.GetType() != typeof(System.Windows.Media.Color)) return null;
            var color = (System.Windows.Media.Color)value;
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }
    }
}