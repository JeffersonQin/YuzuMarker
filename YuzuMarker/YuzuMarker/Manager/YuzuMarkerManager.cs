using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using YuzuMarker.Files;

namespace YuzuMarker.Manager
{
    public static class YuzuMarkerManager
    {
        private static YuzuProject<ObservableCollection<YuzuImage<ObservableCollection<YuzuNotationGroup>>>, 
            ObservableCollection<YuzuNotationGroup>> _Project = null;

        public static YuzuProject<ObservableCollection<YuzuImage<ObservableCollection<YuzuNotationGroup>>>,
            ObservableCollection<YuzuNotationGroup>> Project {
            get
            {
                return _Project;
            }
            set
            {
                _Project = value;
            }
        }

        private static YuzuImage<ObservableCollection<YuzuNotationGroup>> _Image = null;

        public static YuzuImage<ObservableCollection<YuzuNotationGroup>> Image
        {
            get
            {
                return _Image;
            }
            set
            {
                _Image = value;
            }
        }
    }
}
