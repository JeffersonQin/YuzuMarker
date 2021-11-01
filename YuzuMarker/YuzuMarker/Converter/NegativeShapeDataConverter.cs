using System;
using System.Globalization;
using System.Windows.Data;
using YuzuMarker.Model;

namespace YuzuMarker.Converter
{
    public class ShapeData2TopConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var data = value as ShapeData;
            if (data == null) return 0;
            if (data.Height < 0) return data.Y + data.Height;
            return data.Y;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ShapeData2LeftConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var data = value as ShapeData;
            if (data == null) return 0;
            if (data.Width < 0) return data.X + data.Width;
            return data.X;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
    public class NegativeLengthDataConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Math.Abs((float)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}