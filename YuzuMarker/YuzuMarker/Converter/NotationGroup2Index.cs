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
            return Manager.YuzuMarkerManager.Image.NotationGroups.IndexOf((YuzuNotationGroup)value) + 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (((int)value) - 1 < Manager.YuzuMarkerManager.Image.NotationGroups.Count)
                return Manager.YuzuMarkerManager.Image.NotationGroups[((int)value) - 1];
            return null;
        }
    }
}
