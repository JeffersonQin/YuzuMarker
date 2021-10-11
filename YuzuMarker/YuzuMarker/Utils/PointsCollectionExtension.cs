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
    }
}