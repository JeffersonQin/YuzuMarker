using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using YuzuMarker.DataFormat;

namespace YuzuMarker.Converter
{
    public class NotationGroup2Index : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var notationGroup = (YuzuNotationGroup)values[0];
            return notationGroup?.ParentImage.NotationGroups.IndexOf(notationGroup) + 1;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
