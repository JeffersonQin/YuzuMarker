using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Media;

namespace YuzuMarker.Utils
{
    public static class PointsCollectionExtension
    {
        public static List<PointF> ToGenericPoints(this PointCollection pointCollection)
        {
            List<PointF> points = new List<PointF>();
            foreach (var p in pointCollection)
                points.Add(new PointF((float)p.X, (float)p.Y));
            return points;
        }

        public static PointCollection ToPointCollection(this List<PointF> points)
        {
            PointCollection pointCollection = new PointCollection();
            points.ForEach(p => pointCollection.Add(new System.Windows.Point(p.X, p.Y)));
            return pointCollection;
        }

        public static OpenCvSharp.Point[] ToOpenCvPoint(this PointCollection pointCollection)
        {
            OpenCvSharp.Point[] points = new OpenCvSharp.Point[pointCollection.Count];
            for (var i = 0; i < pointCollection.Count; i++)
                points[i] = new OpenCvSharp.Point((int)pointCollection[i].X, (int)pointCollection[i].Y);
            return points;
        }
    }
}