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
            
            // TODO: refactor start: 逻辑将修改为 : cv::Mat (1-channel) => WPF (Writable Bitmap)
            // 如果 YuzuManager.NotationGroup == notationGroup, 用特殊颜色显示，settings里加一下
            if (!notationGroup.CleaningNotation.IsEmpty())
            {
                Scalar color = default;
                if (notationGroup.CleaningNotation.CleaningNotationType == YuzuCleaningNotationType.Normal)
                    color = Settings.Default.NormalCleaningFillColor.ToScalar();
                if (notationGroup.CleaningNotation.CleaningNotationType == YuzuCleaningNotationType.Custom)
                    color = Settings.Default.CustomCleaningFillColor.ToScalar();
                // Create 4 channel output image with color on
                var backgroundMat = new Mat(new Size(notationGroup.CleaningNotation.CleaningMask.Cols, 
                        notationGroup.CleaningNotation.CleaningMask.Rows), MatType.CV_8UC4, color);
                // Create mask array, used for merging
                var maskArray = new Mat[4];
                // Get Mat of 1 Channel mask
                var c1Mask = notationGroup.CleaningNotation.CleaningMask.GetMat(AccessFlag.READ);
                // Assign mask array
                for (var i = 0; i < 4; i ++)
                    maskArray[i] = c1Mask;
                // Create 4 channel mask instance
                var dstMask = new Mat();
                // Merge mask array into one single mask
                Cv2.Merge(maskArray, dstMask);
                // Create render mat instance
                var outputMat = new Mat();
                // Perform bitwise and to generate rendering mat
                Cv2.BitwiseAnd(backgroundMat, dstMask, outputMat);
                // Convert rendering mat to wpf image source
                var image = new OpaqueClickableImage { Source = outputMat.ToWriteableBitmap() };
                // Dispose mat objects
                outputMat.Dispose();
                dstMask.Dispose();
                c1Mask.Dispose();
                // Add image to container
                container.Children.Add(image);
            }
            // TODO: refactor end

            return container;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
