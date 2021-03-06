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
                notationGroup?.Load();
            }
        }

        public void WriteImageNotations()
        {
            foreach (var basicYuzuNotationGroup in NotationGroups)
            {
                var notationGroup = basicYuzuNotationGroup as YuzuNotationGroup;
                notationGroup?.Write();
            }
        }

        public void UnloadImageNotations()
        {
            foreach (var basicYuzuNotationGroup in NotationGroups)
            {
                var notationGroup = basicYuzuNotationGroup as YuzuNotationGroup;
                notationGroup?.Unload();
            }
        }

        public void CreateAndLoadNewNotationGroup(int x, int y, string text, bool finished)
        {
            var notationGroup = new YuzuNotationGroup(this, x, y, text, finished);
            notationGroup.Load();
            NotationGroups.Add(notationGroup);
        }

        public void RemoveAndUnloadNotationGroup(YuzuNotationGroup notationGroup)
        {
            notationGroup.Unload();
            NotationGroups.Remove(notationGroup);
        }
    }
}
