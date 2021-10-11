using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace YuzuMarker.DataFormat
{
    public class YuzuCleaningNotation
    {
        public YuzuCleaningNotationType CleaningNotationType;

        public List<PointF> CleaningPoints;

        public YuzuCleaningNotation(YuzuCleaningNotationType type, List<PointF> points)
        {
            CleaningNotationType = type;
            CleaningPoints = points;
        }

        public YuzuCleaningNotation(YuzuCleaningNotationType type)
        {
            CleaningNotationType = type;
            CleaningPoints = new List<PointF>();
        }
    }
}
