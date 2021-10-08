using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace YuzuMarker.DataFormat
{
    public class YuzuImage<LI> where LI : IList<YuzuNotationGroup>, new()
    {
        public string ParentPath { get; set; }

        public string ImageName { get; set; }

        public LI NotationGroups { get; set; }

        public bool IsFinished { get; set; }

        public YuzuImage(string ParentPath, string ImageName, bool finished)
        {
            if (!File.Exists(Path.Combine(ParentPath, "./Images/", ImageName)))
                throw new Exception("YuzuImage Init Error: file does not exist. Name: " + ImageName);
            this.ParentPath = ParentPath;
            this.ImageName = ImageName;
            NotationGroups = new LI();
            IsFinished = finished;
        }

        public void CreateNewNotation(int x, int y, string text, bool finished)
        {
            NotationGroups.Add(new YuzuNotationGroup(x, y, text, finished));
        }

        public void RemoveNotationGroupAt(int index)
        {
            NotationGroups.RemoveAt(index);
        }

        public void CreateNewNotationAt(int index, int x, int y, string text, bool finished)
        {
            NotationGroups.Insert(index, new YuzuNotationGroup(x, y, text, finished));
        }
        
        public void MoveNotationGroup(int fromIndex, int toIndex)
        {
            YuzuNotationGroup notation = NotationGroups[fromIndex];
            NotationGroups.RemoveAt(fromIndex);
            NotationGroups.Insert(toIndex, notation);
        }

        public string GetImageFilePath()
        {
            YuzuProject.EnsureImageFolderExist(ParentPath);
            return Path.Combine(ParentPath, "./Images/", ImageName);
        }

        public string GetImagePSDPath()
        {
            YuzuProject.EnsurePSDFolderExist(ParentPath);
            return Path.Combine(ParentPath, "./PSD/" + ImageName + ".psd");
        }
    }
}
