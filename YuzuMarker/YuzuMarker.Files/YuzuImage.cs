using System;
using System.Collections.Generic;
using System.Text;

namespace YuzuMarker.Files
{
    public class YuzuImage
    {
        private string Path;

        public YuzuImage(string Path)
        {
            SetPath(Path);
        }

        public void SetPath(string Path)
        {
            if (!IOUtils.JudgeFilePath(Path))
                throw new Exception("Invalid Image File Path");
            this.Path = Path;
        }

        public string GetPath()
        {
            return Path;
        }
    }
}
