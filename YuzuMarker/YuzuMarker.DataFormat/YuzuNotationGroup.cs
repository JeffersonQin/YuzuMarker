using System;
using System.Collections.Generic;
using System.Text;

namespace YuzuMarker.DataFormat
{
    public class YuzuNotationGroup
    {
        public long Timestamp { get; set; }

        public int x { get; set; }

        public int y { get; set; }

        public string text { get; set; }

        public bool IsFinished { get; set; }
        
        public YuzuCleaningNotation CleaningNotation { get; set; }

        public YuzuNotationGroup(int x, int y, string text, bool finished)
        {
            Timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            this.x = x;
            this.y = y;
            this.text = text;
            IsFinished = finished;
            CleaningNotation = new YuzuCleaningNotation(YuzuCleaningNotationType.None);
        }

        public YuzuNotationGroup(long Timestamp, int x, int y, string text, bool finished)
        {
            this.Timestamp = Timestamp;
            this.x = x;
            this.y = y;
            this.text = text;
            IsFinished = finished;
            CleaningNotation = new YuzuCleaningNotation(YuzuCleaningNotationType.None);
        }

        // Other kinds of notations
    }
}
