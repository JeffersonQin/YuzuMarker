using System;
using System.Collections.Generic;
using System.IO;

namespace YuzuMarker.Files
{
    public class YuzuProject
    {
        private string _path;

        public string path
        {
            get
            {
                return _path;
            }
            set
            {
                if (!IOUtils.JudgeFilePath(value))
                    throw new Exception("Invalid Yuzu Project Path");
                _path = value;
            }
        }

        public string projectName;

        private string _fileName;

        public string fileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                if (!IOUtils.JudgeFileName(value))
                    throw new Exception("Invalid Yuzu Project file name");
                _fileName = value;
            }
        }

        public List<YuzuImage> Images;

        public void EnsureImageFolderExist()
        {
            IOUtils.EnsureDirectoryExist(Path.Combine(path, "./Images"));
        }

        public void EnsurePSDFolderExist()
        {
            IOUtils.EnsureDirectoryExist(Path.Combine(path, "./PSD"));
        }

        public YuzuProject(string path, string fileName, string projectName)
        {
            this.path = path;
            this.fileName = fileName;
            this.projectName = projectName;
            Images = new List<YuzuImage>();
        }

        public YuzuProject(string path, string fileName, string projectName, List<YuzuImage> Images)
        {
            this.path = path;
            this.fileName = fileName;
            this.projectName = projectName;
            this.Images = Images;
        }

        public void RemoveImageAt(int index)
        {
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

            string targetImagePath = Path.Combine(path, "./Images/", imageFileName);
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
