using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using YuzuMarker.DataFormat;
using YuzuMarker.Utils;

namespace YuzuMarker.Converter
{
    public class NotationGroup2CanvasItem : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Grid container =  new Grid();

            YuzuNotationGroup notationGroup = (YuzuNotationGroup)value;

            if (notationGroup.CleaningNotation.CleaningNotationType != YuzuCleaningNotationType.None)
            {
                Polygon cleaningPolygon = new Polygon();
                cleaningPolygon.Points = notationGroup.CleaningNotation.CleaningPoints.ToPointCollection();
                if (notationGroup.CleaningNotation.CleaningNotationType == YuzuCleaningNotationType.Custom)
                    cleaningPolygon.Stroke = new SolidColorBrush(Colors.Red);
                else
                    cleaningPolygon.Stroke = new SolidColorBrush(Colors.Blue);

                container.Children.Add(cleaningPolygon);
            }

            return container;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
