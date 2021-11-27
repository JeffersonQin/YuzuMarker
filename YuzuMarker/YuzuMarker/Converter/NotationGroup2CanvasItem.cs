using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using OpenCvSharp;
using OpenCvSharp.Internal;
using OpenCvSharp.WpfExtensions;
using YuzuMarker.Control;
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
            
            // TODO: refactor: 重写 cleaning 的渲染
            // TODO: 如果 YuzuManager.NotationGroup == notationGroup, 用特殊颜色显示，settings里加一下
            Scalar color = default;
            if (notationGroup.CleaningNotation.CleaningNotationType == YuzuCleaningNotationType.Color)
                color = ((YuzuColorCleaningNotation)notationGroup.CleaningNotation).CleaningNotationColor.ToScalar();
            if (notationGroup.CleaningNotation.CleaningNotationType == YuzuCleaningNotationType.Impainting)
                color = Settings.Default.CustomCleaningFillColor.ToScalar();
            // TODO: 实现边缘获取
            container.Children.Add(new OpaqueClickableImage
            {
                Source = notationGroup.CleaningNotation.CleaningMask.To4ChannelImage(color)
            });

            return container;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
