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
    public class NotationGroup2CanvasItem : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Grid container =  new Grid();

            YuzuNotationGroup notationGroup = (YuzuNotationGroup)value;
            if (notationGroup == null) return null;
            
            if (notationGroup.CleaningNotation.CleaningPoints.Count > 0)
            {
                Polygon cleaningPolygon = new Polygon();
                cleaningPolygon.Points = notationGroup.CleaningNotation.CleaningPoints.ToPointCollection();
                cleaningPolygon.StrokeThickness = Settings.Default.UIPolygonStrokeThickness;
                if (notationGroup.CleaningNotation.CleaningNotationType == YuzuCleaningNotationType.Custom)
                {
                    cleaningPolygon.Stroke = new SolidColorBrush(Settings.Default.CustomCleaningStrokeColor.ToColor());
                    cleaningPolygon.Fill = new SolidColorBrush(Settings.Default.CustomCleaningFillColor.ToColor());
                }
                else
                {
                    cleaningPolygon.Stroke = new SolidColorBrush(Settings.Default.NormalCleaningStrokeColor.ToColor());
                    cleaningPolygon.Fill = new SolidColorBrush(Settings.Default.NormalCleaningFillColor.ToColor());
                }

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
