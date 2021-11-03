using System;
using System.Globalization;
using System.Windows.Data;
using YuzuMarker.Model;

namespace YuzuMarker.Converter
{
    public class SelectionTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null) return false;
            if ((int)value == int.Parse(parameter.ToString() ?? "-1")) return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value) return (SelectionType)int.Parse(parameter.ToString());
            return null;
        }
    }
}