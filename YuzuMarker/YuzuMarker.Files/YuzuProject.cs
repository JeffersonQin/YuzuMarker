using System;
using System.Collections.Generic;
using System.IO;

namespace YuzuMarker.Files
{
    public class YuzuProject
    {
        private string path;

        private string name;

        public List<YuzuImage> Images;

        public YuzuProject(string name, string path)
        {
            SetName(name);
            SetPath(path);
            Images = new List<YuzuImage>();
        }

        public YuzuProject(string name, string path, List<YuzuImage> Images)
        {
            SetName(name);
            SetPath(path);
            this.Images = Images;
        }

        public string GetName()
        {
            return name;
        }

        public void SetName(string name)
        {
            if (!IOUtils.JudgeFileName(name))
                throw new Exception("Invalid Yuzu Project Name");
            this.name = name;
        }

        public string GetPath()
        {
            return path;
        }

        public void SetPath(string path)
        {
            if (!IOUtils.JudgeFilePath(path))
                throw new Exception("Invalid Yuzu Project Path");
            this.path = path;
        }

        public string GetImagePathAt(int index)
        {
            return Path.Combine(path, Images[index].GetPath());
        }

        public string GetImageNotationPathAt(int index)
        {
            return GetImagePathAt(index) + ".yznt";
        }

        public string GetImagePSDPathAt(int index)
        {
            return GetImagePathAt(index) + ".psd";
        }
    }
}
