using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace YuzuMarker.Converter
{
    public class IntPlusConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return 0;
            if (parameter == null) parameter = 0;

            if (int.TryParse(value.ToString(), out int number) && int.TryParse(parameter.ToString(), out int param))
                return number + param;
            return 0;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return 0;
            if (parameter == null) parameter = 0;

            if (int.TryParse(value.ToString(), out int number) && int.TryParse(parameter.ToString(), out int param))
                return number - param;
            return 0;
        }
    }
}
