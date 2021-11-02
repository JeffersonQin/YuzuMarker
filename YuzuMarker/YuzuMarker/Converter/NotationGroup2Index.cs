using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using YuzuMarker.DataFormat;

namespace YuzuMarker.Converter
{
    public class NotationGroup2Index : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var notationGroup = (YuzuNotationGroup)value;
            return notationGroup?.ParentImage.NotationGroups.IndexOf(notationGroup) + 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
