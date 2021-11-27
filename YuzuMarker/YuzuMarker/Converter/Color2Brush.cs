using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace YuzuMarker.Converter
{
    /// <summary>
    /// Modified from: https://github.com/PixiEditor/ColorPicker/blob/master/src/ColorPicker/Converters/ColorToBrushConverter.cs
    /// </summary>
    [ValueConversion(typeof(Color), typeof(SolidColorBrush))]
    public class Color2Brush : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color col = (Color)value;
            Color c = Color.FromArgb(col.A, col.R, col.G, col.B);
            return new SolidColorBrush(c);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SolidColorBrush c = (SolidColorBrush)value;
            Color col = Color.FromArgb(c.Color.A, c.Color.R, c.Color.G, c.Color.B);
            return col;
        }
    }
}