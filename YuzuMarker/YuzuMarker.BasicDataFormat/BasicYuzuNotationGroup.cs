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
            set => SetProperty(ref _parentImage, value);
        }
        
        private long _timestamp;

        public long Timestamp
        {
            get => _timestamp;
            set => SetProperty(ref _timestamp, value, beforeChanged: () =>
            {
                UndoRedoManager.PushRecord(Timestamp, (o) =>
                {
                    Timestamp = (long)o;
                }, () => Timestamp);
            });
        }

        private int _x;

        public int X
        {
            get => _x;
            set => SetProperty(ref _x, value, beforeChanged: () =>
            {
                UndoRedoManager.PushRecord(X, (o) =>
                {
                    X = (int)o;
                }, () => X);
            });
        }

        private int _y;

        public int Y
        {
            get => _y;
            set => SetProperty(ref _y, value, beforeChanged: () =>
            {
                UndoRedoManager.PushRecord(Y, (o) =>
                {
                    Y = (int)o;
                }, () => Y);
            });
        }

        private string _text;

        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value, beforeChanged: () =>
            {
                UndoRedoManager.PushRecord(Text, (o) =>
                {
                    Text = (string)o;
                }, () => Text);
            });
        }

        private bool _isFinished;

        public bool IsFinished
        {
            get => _isFinished;
            set => SetProperty(ref _isFinished, value, beforeChanged: () =>
            {
                UndoRedoManager.PushRecord(IsFinished, (o) =>
                {
                    IsFinished = (bool)o;
                }, () => IsFinished);
            });
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
