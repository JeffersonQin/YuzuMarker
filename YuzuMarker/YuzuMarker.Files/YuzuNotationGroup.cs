using System;
using System.Collections.Generic;
using System.Text;

namespace YuzuMarker.Files
{
    public class YuzuNotationGroup
    {
        public long Timestamp { get; set; }

        public int x { get; set; }

        public int y { get; set; }

        public string text { get; set; }

        public YuzuNotationGroup(int x, int y, string text)
        {
            Timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            this.x = x;
            this.y = y;
            this.text = text;
        }

        public YuzuNotationGroup(long Timestamp, int x, int y, string text)
        {
            this.Timestamp = Timestamp;
            this.x = x;
            this.y = y;
            this.text = text;
        }

        // Other kinds of notations
    }
}
