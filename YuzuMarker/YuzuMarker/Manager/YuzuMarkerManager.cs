using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using YuzuMarker.Common;
using YuzuMarker.DataFormat;

namespace YuzuMarker.Manager
{
    public static class YuzuMarkerManager
    {
        public static YuzuProject Project { get; set; }

        public static YuzuImage Image { get; set; }

        public static YuzuNotationGroup Group { get; set; }

        public static ObservableCollection<string> MessageStack { get; } = new ObservableCollection<string>() { "就绪" };

        public static void PushMessage(NotifyObject dataContext, string message)
        {
            MessageStack.Add(message);
            dataContext.RaisePropertyChanged("TopMessage");
        }

        public static string PopMessage(NotifyObject dataContext)
        {
            var lastMessage = MessageStack[^1];
            MessageStack.RemoveAt(MessageStack.Count - 1);
            dataContext.RaisePropertyChanged("TopMessage");
            return lastMessage;
        }
    }
}
