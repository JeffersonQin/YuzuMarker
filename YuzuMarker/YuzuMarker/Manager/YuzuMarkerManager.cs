using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using YuzuMarker.DataFormat;

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

        private static YuzuNotationGroup _Group = null;

        public static YuzuNotationGroup Group
        {
            get
            {
                return _Group;
            }
            set
            {
                _Group = value;
            }
        }

        private static ObservableCollection<string> _MessageStack = new ObservableCollection<string>() { "就绪" };

        public static ObservableCollection<string> MessageStack
        {
            get
            {
                return _MessageStack;
            }
            set
            {
                _MessageStack = value;
            }
        }

        public static void PushMessage(NotifyObject dataContext, string message)
        {
            MessageStack.Add(message);
            dataContext.RaisePropertyChanged("TopMessage");
        }

        public static string PopMessage(NotifyObject dataContext)
        {
            string lastMessage = MessageStack[^1];
            MessageStack.RemoveAt(MessageStack.Count - 1);
            dataContext.RaisePropertyChanged("TopMessage");
            return lastMessage;
        }
    }
}
