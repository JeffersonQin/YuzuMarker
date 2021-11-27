using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;
using YuzuMarker.DataFormat;
using YuzuMarker.Utils;

namespace YuzuMarker.Converter
{
    public class Cleaning2StatusString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            YuzuCleaningNotation cleaningNotation = (YuzuCleaningNotation) value;
            
            var statusString = cleaningNotation.CleaningMask.IsEmpty() ? "[未选区]" : "[已选区]";

            switch (cleaningNotation.CleaningNotationType)
            {
                case YuzuCleaningNotationType.Color:
                    if (((YuzuColorCleaningNotation)cleaningNotation).CleaningNotationColor == Color.Transparent)
                        statusString += "[未选色]";
                    else statusString += "[已选色]";
                    break;
                case YuzuCleaningNotationType.Impainting:
                    if (((YuzuImpaintingCleaningNotation)cleaningNotation).ImpaintingImage.IsEmpty())
                        statusString += "[未修复]";
                    else statusString += "[已修复]";
                    break;
            }

            return statusString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}