using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace YuzuMarker.DataFormat
{
    public class YuzuProject
    {
        public static void EnsureImageFolderExist(string parentPath)
        {
            IOUtils.EnsureDirectoryExist(Path.Combine(parentPath, "./Images"));
        }

        public static void EnsurePSDFolderExist(string parentPath)
        {
            IOUtils.EnsureDirectoryExist(Path.Combine(parentPath, "./PSD"));
        }
    }

    public class YuzuProject<LP, LI> where LP : IList<YuzuImage<LI>>, new() where LI : IList<YuzuNotationGroup>, new()
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

        public string projectName { get; set; }

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

        public LP Images { get; set; }

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
            this.Images = new LP();
        }

        public YuzuProject(string path, string fileName, string projectName, LP Images)
        {
            this.path = path;
            this.fileName = fileName;
            this.projectName = projectName;
            this.Images = Images;
        }

        public void RemoveImageAt(int index)
        {
            File.Delete(Path.Combine(path, Images[index].ImageName));
            Images.RemoveAt(index);
        }

        public void RemoveImage(YuzuImage<LI> image)
        {
            File.Delete(Path.Combine(path, image.ImageName));
            Images.Remove(image);
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
                foreach (YuzuImage<LI> yuzuImage in Images)
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
            File.Copy(imagePath, targetImagePath);

            return imageFileName;
        }

        public string CreateNewImageAt(int index, string imagePath)
        {
            string imageFileName = CopyImage(imagePath);

            Images.Insert(index, new YuzuImage<LI>(this.path, imageFileName));
            return imageFileName;
        }

        public string CreateNewImage(string imagePath)
        {
            string imageFileName = CopyImage(imagePath);

            Images.Add(new YuzuImage<LI>(this.path, imageFileName));
            return imageFileName;
        }
    }
}
