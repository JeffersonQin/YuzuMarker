using System;
using System.Collections.Generic;
using System.IO;

namespace YuzuMarker.Files
{
    public class YuzuProject
    {
        private string path;

        public string name;

        private List<YuzuImage> Images;

        public void EnsureImageFolderExist()
        {
            if (!Directory.Exists(Path.Combine(path, "./Images")))
                Directory.CreateDirectory(Path.Combine(path, "./Images"));
        }

        public void EnsurePSDFolderExist()
        {
            if (!Directory.Exists(Path.Combine(path, "./PSD")))
                Directory.CreateDirectory(Path.Combine(path, "./PSD"));
        }

        public YuzuProject(string name, string path)
        {
            SetPath(path);
            this.name = name;
            Images = new List<YuzuImage>();
        }

        public YuzuProject(string name, string path, List<YuzuImage> Images)
        {
            SetPath(path);
            this.name = name;
            this.Images = Images;
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

        public void RemoveImageAt(int index, bool deleteFile)
        {
            if (deleteFile)
                new FileInfo(Path.Combine(path, Images[index].ImageName)).Delete();
            Images.RemoveAt(index);
        }

        private string CopyImage(string imagePath)
        {
            EnsureImageFolderExist();
            string originalImageFileName = Path.GetFileName(imagePath);
            string imageFileName = originalImageFileName;
            int prefix = 1;

            bool exist = true;
            while (exist)
            {
                exist = false;
                foreach (YuzuImage yuzuImage in Images)
                {
                    if (yuzuImage.ImageName == imageFileName)
                    {
                        exist = true;
                        imageFileName = (prefix++) + "-" + originalImageFileName;
                        break;
                    }
                }
            }

            string targetImagePath = Path.Combine(path, imageFileName);
            new FileInfo(imagePath).CopyTo(targetImagePath);

            return imageFileName;
        }

        public string InsertImageAt(int index, string imagePath)
        {
            string imageFileName = CopyImage(imagePath);

            Images.Insert(index, new YuzuImage(this, imageFileName));
            return imageFileName;
        }

        public string AddImage(string imagePath)
        {
            string imageFileName = CopyImage(imagePath);

            Images.Add(new YuzuImage(this, imageFileName));
            return imageFileName;
        }

        public void MoveImage(int fromIndex, int toIndex)
        {
            YuzuImage moveItem = Images[fromIndex];
            Images.Insert(toIndex, moveItem);
        }
    }
}
