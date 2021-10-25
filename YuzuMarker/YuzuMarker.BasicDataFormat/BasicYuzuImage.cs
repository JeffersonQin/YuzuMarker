using System;
using System.Collections.ObjectModel;
using System.IO;
using YuzuMarker.Common;

namespace YuzuMarker.BasicDataFormat
{
    public class BasicYuzuImage : NotifyObject
    {
        private string _parentPath;

        public string ParentPath
        {
            get => _parentPath;
            set => SetProperty(ref _parentPath, value);
        }
        
        private string _imageName;

        public string ImageName
        {
            get => _imageName;
            set => SetProperty(ref _imageName, value);
        }

        private bool _isFinished;

        public bool IsFinished
        {
            get => _isFinished;
            set => SetProperty(ref _isFinished, value);
        }

        private ObservableCollection<BasicYuzuNotationGroup> _notationGroups;

        public ObservableCollection<BasicYuzuNotationGroup> NotationGroups
        {
            get => _notationGroups;
            set => SetProperty(ref _notationGroups, value);
        }

        public BasicYuzuImage(string parentPath, string imageName, bool finished)
        {
            if (!File.Exists(Path.Combine(parentPath, "./Images/", imageName)))
                throw new Exception("YuzuImage Init Error: file does not exist. Name: " + imageName);
            ParentPath = parentPath;
            ImageName = imageName;
            NotationGroups = new ObservableCollection<BasicYuzuNotationGroup>();
            IsFinished = finished;
        }
        
        public virtual void CreateNewNotation(int x, int y, string text, bool finished)
        {
            NotationGroups.Add(new BasicYuzuNotationGroup(x, y, text, finished));
        }

        public void RemoveNotationGroupAt(int index)
        {
            NotationGroups.RemoveAt(index);
        }

        public virtual void CreateNewNotationAt(int index, int x, int y, string text, bool finished)
        {
            NotationGroups.Insert(index, new BasicYuzuNotationGroup(x, y, text, finished));
        }
        
        public void MoveNotationGroup(int fromIndex, int toIndex)
        {
            var notation = NotationGroups[fromIndex];
            NotationGroups.RemoveAt(fromIndex);
            NotationGroups.Insert(toIndex, notation);
        }

        public string GetImageFilePath()
        {
            BasicYuzuProject.EnsureImageFolderExist(ParentPath);
            return Path.Combine(ParentPath, "./Images/", ImageName);
        }

        public string GetImagePsdPath()
        {
            BasicYuzuProject.EnsurePsdFolderExist(ParentPath);
            return Path.Combine(ParentPath, "./PSD/" + ImageName + ".psd");
        }
    }
}
