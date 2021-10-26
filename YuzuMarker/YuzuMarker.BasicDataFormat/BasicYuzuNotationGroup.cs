﻿using System;
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
            set => SetProperty(ref _timestamp, value);
        }

        private int _x;

        public int X
        {
            get => _x;
            set => SetProperty(ref _x, value);
        }

        private int _y;

        public int Y
        {
            get => _y;
            set => SetProperty(ref _y, value);
        }

        private string _text;

        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }

        private bool _isFinished;

        public bool IsFinished
        {
            get => _isFinished;
            set => SetProperty(ref _isFinished, value);
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
