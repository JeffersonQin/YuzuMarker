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
        public YuzuImage(YuzuProject parentProject, string imageName, bool finished) : base(parentProject, imageName, finished) {}

        public override void CreateNewNotation(int x, int y, string text, bool finished)
        {
            NotationGroups.Add(new YuzuNotationGroup(this, x, y, text, finished));
        }

        public override void CreateNewNotationAt(int index, int x, int y, string text, bool finished)
        {
            NotationGroups.Insert(index, new YuzuNotationGroup(this, x, y, text, finished));
        }

        public void LoadImageNotations()
        {
            foreach (var basicYuzuNotationGroup in NotationGroups)
            {
                var notationGroup = basicYuzuNotationGroup as YuzuNotationGroup;
                notationGroup?.LoadNotationResource();
            }
        }

        public void WriteImageNotations()
        {
            foreach (var basicYuzuNotationGroup in NotationGroups)
            {
                var notationGroup = basicYuzuNotationGroup as YuzuNotationGroup;
                notationGroup?.WriteNotationResource();
            }
        }

        public void UnloadImageNotations()
        {
            foreach (var basicYuzuNotationGroup in NotationGroups)
            {
                var notationGroup = basicYuzuNotationGroup as YuzuNotationGroup;
                notationGroup?.UnloadNotationResource();
            }
        }
    }
}
