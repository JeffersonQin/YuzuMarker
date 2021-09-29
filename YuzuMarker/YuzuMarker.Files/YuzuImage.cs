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

        public Dictionary<long, YuzuSimpleNotation> SimpleNotations = new Dictionary<long, YuzuSimpleNotation>();

        public YuzuImage(YuzuProject parent, string ImageName)
        {
            if (!File.Exists(Path.Combine(parent.path, "./Images/", ImageName)))
                throw new Exception("YuzuImage Init Error: file does not exist. Name: " + ImageName);
            this.parent = parent;
            this.ImageName = ImageName;
        }

        public void AddSimpleNotation(int x, int y, string text)
        {
            SimpleNotations.Add(new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds(), new YuzuSimpleNotation(x, y, text));
        }

        public void RemoveNotation(long timestamp)
        {
            SimpleNotations.Remove(timestamp);
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
