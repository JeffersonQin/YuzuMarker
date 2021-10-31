using System;
using System.Globalization;
using System.Windows.Data;
using YuzuMarker.DataFormat;

namespace YuzuMarker.Converter
{
    public class Cleaning2StatusString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            YuzuCleaningNotation cleaningNotation = (YuzuCleaningNotation) value;
            if (cleaningNotation.CleaningNotationType == YuzuCleaningNotationType.Custom) return "PS自定义蒙版";
            if (cleaningNotation.IsEmpty()) return "不涂白";
            return "区域已选择";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}