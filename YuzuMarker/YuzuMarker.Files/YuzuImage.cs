using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace YuzuMarker.Files
{
    public class YuzuImage
    {
        public YuzuProject parent { get; set; }

        public string ImageName { get; set; }

        public List<YuzuNotationGroup> NotationGroups { get; set; }

        public YuzuImage(YuzuProject parent, string ImageName)
        {
            if (!File.Exists(Path.Combine(parent.path, "./Images/", ImageName)))
                throw new Exception("YuzuImage Init Error: file does not exist. Name: " + ImageName);
            this.parent = parent;
            this.ImageName = ImageName;
            NotationGroups = new List<YuzuNotationGroup>();
        }

        public void CreateNewNotation(int x, int y, string text)
        {
            NotationGroups.Add(new YuzuNotationGroup(x, y, text));
        }

        public void RemoveNotationGroupAt(int index)
        {
            NotationGroups.RemoveAt(index);
        }

        public void CreateNewNotationAt(int index, int x, int y, string text)
        {
            NotationGroups.Insert(index, new YuzuNotationGroup(x, y, text));
        }
        
        public void MoveNotationGroup(int fromIndex, int toIndex)
        {
            YuzuNotationGroup notation = NotationGroups[fromIndex];
            NotationGroups.RemoveAt(fromIndex);
            NotationGroups.Insert(toIndex, notation);
        }

        public string GetImageFilePath()
        {
            parent.EnsureImageFolderExist();
            return Path.Combine(parent.path, "./Images/", ImageName);
        }

        public string GetImagePSDPath()
        {
            parent.EnsurePSDFolderExist();
            return Path.Combine(parent.path, "./PSD/" + ImageName + ".psd");
        }
    }
}
