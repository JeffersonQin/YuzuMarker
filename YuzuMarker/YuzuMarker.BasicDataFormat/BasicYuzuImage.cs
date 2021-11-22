using System;
using System.Collections.ObjectModel;
using System.IO;
using YuzuMarker.Common;

namespace YuzuMarker.BasicDataFormat
{
    public class BasicYuzuImage : NotifyObject
    {
        private BasicYuzuProject _parentProject;

        public BasicYuzuProject ParentProject
        {
            get => _parentProject;
            set => SetProperty(ref _parentProject, value);
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
            set => UndoRedoManager.PushAndPerformRecord(o =>
            {
                var nowValue = IsFinished;
                SetProperty(ref _isFinished, (bool)o);
                return nowValue;
            }, o =>
            {
                var nowValue = IsFinished;
                o ??= value;
                SetProperty(ref _isFinished, (bool)o);
                return nowValue;
            });
        }

        private ObservableCollection<BasicYuzuNotationGroup> _notationGroups;

        public ObservableCollection<BasicYuzuNotationGroup> NotationGroups
        {
            get => _notationGroups;
            set => SetProperty(ref _notationGroups, value);
        }

        public BasicYuzuImage(BasicYuzuProject parentProject, string imageName, bool finished)
        {
            if (!File.Exists(Path.Combine(parentProject.Path, "./Images/", imageName)))
                throw new Exception("YuzuImage Init Error: file does not exist. Name: " + imageName);
            ParentProject = parentProject;
            ImageName = imageName;
            NotationGroups = new ObservableCollection<BasicYuzuNotationGroup>();
            IsFinished = finished;
        }
        
        public virtual void CreateNewNotation(int x, int y, string text, bool finished)
        {
            NotationGroups.Add(new BasicYuzuNotationGroup(this, x, y, text, finished));
        }

        public void RemoveNotationGroupAt(int index)
        {
            NotationGroups.RemoveAt(index);
        }

        public void RemoveNotationGroup(BasicYuzuNotationGroup notationGroup)
        {
            NotationGroups.Remove(notationGroup);
        }

        public virtual void CreateNewNotationAt(int index, int x, int y, string text, bool finished)
        {
            NotationGroups.Insert(index, new BasicYuzuNotationGroup(this, x, y, text, finished));
        }
        
        public void MoveNotationGroup(int fromIndex, int toIndex)
        {
            var notation = NotationGroups[fromIndex];
            NotationGroups.RemoveAt(fromIndex);
            NotationGroups.Insert(toIndex, notation);
        }

        public string GetImageFilePath()
        {
            ParentProject.EnsureImageFolderExist();
            return Path.Combine(ParentProject.Path, "./Images/", ImageName);
        }

        public string GetImagePsdPath()
        {
            ParentProject.EnsurePsdFolderExist();
            return Path.Combine(ParentProject.Path, "./PSD/" + ImageName + ".psd");
        }

        public string GetImageTempPath()
        {
            ParentProject.EnsureTempFolderExist();
            var path = Path.Combine(ParentProject.Path, "./temp/" + ImageName);
            IOUtils.EnsureDirectoryExist(path);
            return path;
        }

        public string GetImageNotationPath()
        {
            ParentProject.EnsureImageFolderExist();
            var path = Path.Combine(ParentProject.Path, "./Notations/" + ImageName);
            IOUtils.EnsureDirectoryExist(path);
            return path;
        }
    }
}
