using System;
using System.Collections.ObjectModel;
using System.Linq;
using YuzuMarker.Common;

namespace YuzuMarker.BasicDataFormat
{
    public class BasicYuzuProject : NotifyObject
    {
        private string _path;

        public string Path
        {
            get => _path;
            set
            {
                if (!IOUtils.JudgeFilePath(value))
                    throw new Exception("Invalid Yuzu Project Path");
                SetProperty(ref _path, value);
            }
        }

        private string _projectName;

        public string ProjectName
        {
            get => _projectName;
            set => SetProperty(ref _projectName, value);
        }
        
        private string _fileName;

        public string FileName
        {
            get => _fileName;
            set
            {
                if (!IOUtils.JudgeFileName(value))
                    throw new Exception("Invalid Yuzu Project file name");
                SetProperty(ref _fileName, value);
            }
        }

        private ObservableCollection<BasicYuzuImage> _images;

        public ObservableCollection<BasicYuzuImage> Images
        {
            get => _images;
            set => SetProperty(ref _images, value);
        }
        
        public void EnsureImageFolderExist()
        {
            IOUtils.EnsureDirectoryExist(System.IO.Path.Combine(Path, "./Images"));
        }
        
        public void EnsurePsdFolderExist()
        {
            IOUtils.EnsureDirectoryExist(System.IO.Path.Combine(Path, "./PSD"));
        }
        
        public BasicYuzuProject(string path, string fileName, string projectName)
        {
            Path = path;
            FileName = fileName;
            ProjectName = projectName;
            Images = new ObservableCollection<BasicYuzuImage>();
        }
        
        public BasicYuzuProject(string path, string fileName, string projectName, ObservableCollection<BasicYuzuImage> images)
        {
            Path = path;
            FileName = fileName;
            ProjectName = projectName;
            Images = images;
        }
        
        public void RemoveImageAt(int index)
        {
            System.IO.File.Delete(System.IO.Path.Combine(Path, Images[index].ImageName));
            Images.RemoveAt(index);
        }
        
        public void RemoveImage(BasicYuzuImage image)
        {
            System.IO.File.Delete(System.IO.Path.Combine(Path, image.ImageName));
            Images.Remove(image);
        }
        
        protected string CopyImage(string imagePath)
        {
            EnsureImageFolderExist();
            var originalImageFileName = System.IO.Path.GetFileName(imagePath);
            var imageFileName = originalImageFileName;
            var prefix = 1;

            var exist = true;
            while (exist)
            {
                exist = false;
                if (Images.All(yuzuImage => yuzuImage.ImageName != imageFileName)) continue;
                exist = true;
                imageFileName = (prefix++) + "-" + originalImageFileName;
            }

            var targetImagePath = System.IO.Path.Combine(Path, "./Images/", imageFileName);
            System.IO.File.Copy(imagePath, targetImagePath);

            return imageFileName;
        }
        
        public virtual string CreateNewImageAt(int index, string imagePath)
        {
            var imageFileName = CopyImage(imagePath);

            Images.Insert(index, new BasicYuzuImage(this, imageFileName, false));
            return imageFileName;
        }
        
        public virtual string CreateNewImage(string imagePath)
        {
            var imageFileName = CopyImage(imagePath);

            Images.Add(new BasicYuzuImage(this, imageFileName, false));
            return imageFileName;
        }
    }
}
