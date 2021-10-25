using System;
using System.Collections.Generic;
using System.Text;
using YuzuMarker.BasicDataFormat;

namespace YuzuMarker.DataFormat
{
    public class YuzuNotationGroup : BasicYuzuNotationGroup
    {
        public YuzuCleaningNotation CleaningNotation { get; set; }

        public YuzuNotationGroup(int x, int y, string text, bool finished) : base(x, y, text, finished)
        {
            CleaningNotation = new YuzuCleaningNotation(YuzuCleaningNotationType.Normal);
        }

        public YuzuNotationGroup(long timestamp, int x, int y, string text, bool finished) : base(timestamp, x, y, text, finished)
        {
            CleaningNotation = new YuzuCleaningNotation(YuzuCleaningNotationType.Normal);
        }

        // Other kinds of notations
    }
}
