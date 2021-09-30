using System;
using System.Collections.Generic;
using System.Text;

namespace YuzuMarker.Files
{
    public class YuzuSimpleNotation
    {
        public int x { get; set; }

        public int y { get; set; }

        public string text { get; set; }

        public YuzuSimpleNotation(int x, int y, string text)
        {
            this.x = x;
            this.y = y;
            this.text = text;
        }
    }
}
