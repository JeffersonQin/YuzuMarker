using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace YuzuMarker.Converter
{
    public class SelectionControllerVisibility : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var selectionModeEnabled = (bool)values[0];
            var selectionDrawing = (bool)values[1];

            if (selectionModeEnabled && (!selectionDrawing))
                return Visibility.Visible;
            return Visibility.Hidden;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}