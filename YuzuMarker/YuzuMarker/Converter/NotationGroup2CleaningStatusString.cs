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
    public class NotationGroup2CleaningStatusString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            YuzuNotationGroup notationGroup = (YuzuNotationGroup)value;
            if (notationGroup == null) return null;

            if (notationGroup.CleaningNotation.CleaningMask.IsEmpty()) return " [ 未选区域 ] ";
            return " [ 已选区域 ] ";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
