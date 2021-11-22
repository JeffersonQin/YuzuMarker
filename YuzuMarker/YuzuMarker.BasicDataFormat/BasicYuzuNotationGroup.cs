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
            set => UndoRedoManager.PushAndPerformRecord(o =>
            {
                var nowValue = Timestamp;
                SetProperty(ref _timestamp, (long)o);
                return nowValue;
            }, o =>
            {
                var nowValue = Timestamp;
                o ??= value;
                SetProperty(ref _timestamp, (long)o);
                return nowValue;
            });
        }

        private int _x;

        public int X
        {
            get => _x;
            set => UndoRedoManager.PushAndPerformRecord(o =>
            {
                var nowValue = X;
                SetProperty(ref _x, (int)o);
                return nowValue;
            }, o =>
            {
                var nowValue = X;
                o ??= value;
                SetProperty(ref _x, (int)o);
                return nowValue;
            });
        }

        private int _y;

        public int Y
        {
            get => _y;
            set => UndoRedoManager.PushAndPerformRecord(o =>
            {
                var nowValue = Y;
                SetProperty(ref _y, (int)o);
                return nowValue;
            }, o =>
            {
                var nowValue = Y;
                o ??= value;
                SetProperty(ref _y, (int)o);
                return nowValue;
            });
        }

        private string _text;

        public string Text
        {
            get => _text;
            set => UndoRedoManager.PushAndPerformRecord(o =>
            {
                var nowValue = Text;
                SetProperty(ref _text, (string)o);
                return nowValue;
            }, o =>
            {
                var nowValue = Text;
                o ??= value;
                SetProperty(ref _text, (string)o);
                return nowValue;
            });
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
