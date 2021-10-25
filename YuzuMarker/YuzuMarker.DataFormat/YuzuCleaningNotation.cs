using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using YuzuMarker.Common;

namespace YuzuMarker.DataFormat
{
    public class YuzuCleaningNotation : NotifyObject
    {
        private YuzuCleaningNotationType _cleaningNotationType;

        public YuzuCleaningNotationType CleaningNotationType
        {
            get => _cleaningNotationType;
            set => SetProperty(ref _cleaningNotationType, value);
        }

        private List<PointF> _cleaningPoints;

        public List<PointF> CleaningPoints
        {
            get => _cleaningPoints;
            set => SetProperty(ref _cleaningPoints, value);
        }

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
