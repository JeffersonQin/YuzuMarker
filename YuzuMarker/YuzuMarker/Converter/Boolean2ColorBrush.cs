using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using YuzuMarker.Utils;

namespace YuzuMarker.Converter
{
    public class Boolean2ColorBrush : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (((bool) value) == true)
            {
                return new SolidColorBrush(Properties.Settings.Default.SuccessStatusColor.ToColor());
            }
            return new SolidColorBrush(Properties.Settings.Default.FailStatusColor.ToColor());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
