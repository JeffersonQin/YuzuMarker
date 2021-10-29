using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using OpenCvSharp;
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

        public Mat CleaningMask;

        public YuzuCleaningNotation(YuzuCleaningNotationType type)
        {
            CleaningNotationType = type;
        }
    }
}
