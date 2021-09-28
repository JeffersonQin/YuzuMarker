using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace YuzuMarker.Files
{
    public class YuzuImage
    {
        public YuzuProject parent;

        public string ImageName;

        public Dictionary<long, YuzuSimpleNotation> Notations = new Dictionary<long, YuzuSimpleNotation>();

        public YuzuImage(YuzuProject parent, string ImageName)
        {
            if (!new FileInfo(Path.Combine(parent.GetPath(), ImageName)).Exists)
                throw new Exception("YuzuImage Init Error: file does not exist. Name: " + ImageName);
            this.parent = parent;
            this.ImageName = ImageName;
        }

        public void AddNotation(int x, int y, string text)
        {
            Notations.Add(new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds(), new YuzuSimpleNotation(x, y, text));
        }

        public void RemoveNotation(long timestamp)
        {
            Notations.Remove(timestamp);
        }

        public string GetImageFilePath()
        {
            parent.EnsureImageFolderExist();
            return Path.Combine(parent.GetPath(), "./Images/", ImageName);
        }

        public string GetImagePSDPath()
        {
            parent.EnsurePSDFolderExist();
            return Path.Combine(parent.GetPath(), "./PSD/" + ImageName + ".psd");
        }
    }
}
