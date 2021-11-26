using System;
using System.Collections.Generic;
using System.Text;
using YuzuMarker.Common;

namespace YuzuMarker.BasicDataFormat
{
    public class BasicYuzuNotationGroup : NotifyObject
    {
        private BasicYuzuImage _parentImage;

        public BasicYuzuImage ParentImage
        {
            get => _parentImage;
            set => SetProperty(value);
        }
        
        private long _timestamp;

        [Undoable]
        public long Timestamp
        {
            get => _timestamp;
            set => SetProperty(value);
        }

        private int _x;

        [Undoable]
        public int X
        {
            get => _x;
            set => SetProperty(value);
        }

        private int _y;

        [Undoable]
        public int Y
        {
            get => _y;
            set => SetProperty(value);
        }

        private string _text;

        [Undoable]
        public string Text
        {
            get => _text;
            set => SetProperty(value);
        }

        private bool _isFinished;

        [Undoable]
        public bool IsFinished
        {
            get => _isFinished;
            set => SetProperty(value);
        }

        public BasicYuzuNotationGroup(BasicYuzuImage parentImage, int x, int y, string text, bool finished)
        {
            ParentImage = parentImage;
            Timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            X = x;
            Y = y;
            Text = text;
            IsFinished = finished;
        }

        public BasicYuzuNotationGroup(BasicYuzuImage parentImage, long timestamp, int x, int y, string text, bool finished)
        {
            ParentImage = parentImage;
            Timestamp = timestamp;
            X = x;
            Y = y;
            Text = text;
            IsFinished = finished;
        }
    }
}
