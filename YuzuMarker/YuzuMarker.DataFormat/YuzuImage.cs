using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using YuzuMarker.BasicDataFormat;

namespace YuzuMarker.DataFormat
{
    public class YuzuImage : BasicYuzuImage
    {
        public YuzuImage(string parentPath, string imageName, bool finished) : base(parentPath, imageName, finished) {}

        public override void CreateNewNotation(int x, int y, string text, bool finished)
        {
            NotationGroups.Add(new YuzuNotationGroup(x, y, text, finished));
        }

        public override void CreateNewNotationAt(int index, int x, int y, string text, bool finished)
        {
            NotationGroups.Insert(index, new YuzuNotationGroup(x, y, text, finished));
        }
    }
}
