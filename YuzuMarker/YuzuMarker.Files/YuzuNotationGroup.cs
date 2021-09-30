using System;
using System.Collections.Generic;
using System.Text;

namespace YuzuMarker.Files
{
    public class YuzuNotationGroup
    {
        public long Timestamp { get; set; }

        public YuzuSimpleNotation SimpleNotation { get; set; }

        public YuzuNotationGroup(int x, int y, string text)
        {
            Timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            SimpleNotation = new YuzuSimpleNotation(x, y, text);
        }

        public YuzuNotationGroup(long Timestamp, int x, int y, string text)
        {
            this.Timestamp = Timestamp;
            SimpleNotation = new YuzuSimpleNotation(x, y, text);
        }

        public YuzuNotationGroup(long Timestamp, YuzuSimpleNotation SimpleNotation)
        {
            this.Timestamp = Timestamp;
            this.SimpleNotation = SimpleNotation;
        }

        // Other kinds of notations
    }
}
