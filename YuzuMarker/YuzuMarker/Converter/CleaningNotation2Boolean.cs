using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;
using YuzuMarker.DataFormat;

namespace YuzuMarker.Converter
{
    public class CleaningNotation2Boolean4Color : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return false;
            return ((YuzuCleaningNotation)value).CleaningNotationType == YuzuCleaningNotationType.Color;
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
    public class CleaningNotation2Boolean4Impainting : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return false;
            return ((YuzuCleaningNotation)value).CleaningNotationType == YuzuCleaningNotationType.Impainting;
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
