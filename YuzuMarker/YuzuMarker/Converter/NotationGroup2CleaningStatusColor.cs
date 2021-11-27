using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using YuzuMarker.DataFormat;
using YuzuMarker.Properties;
using YuzuMarker.Utils;

namespace YuzuMarker.Converter
{
    public class NotationGroup2CleaningStatusColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            YuzuNotationGroup notationGroup = (YuzuNotationGroup)value;
            if (notationGroup == null) return null;

            if (notationGroup.CleaningNotation.CleaningMask.IsEmpty()) return new SolidColorBrush(Settings.Default.FailStatusColor.ToColor());
            return new SolidColorBrush(Settings.Default.SuccessStatusColor.ToColor());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
