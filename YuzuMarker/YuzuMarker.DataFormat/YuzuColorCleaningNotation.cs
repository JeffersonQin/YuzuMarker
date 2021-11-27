﻿using System.Drawing;
using YuzuMarker.Common;

namespace YuzuMarker.DataFormat
{
    public class YuzuColorCleaningNotation : YuzuCleaningNotation
    {
        public YuzuColorCleaningNotation(YuzuNotationGroup parentNotationGroup) : 
            base(parentNotationGroup, YuzuCleaningNotationType.Color) {}
        
        public YuzuColorCleaningNotation(YuzuNotationGroup parentNotationGroup, Color color) : 
            base(parentNotationGroup, YuzuCleaningNotationType.Color)
        {
            CleaningNotationColor = color;
        }

        private Color _cleaningNotationColor = Color.White;

        [Undoable]
        public Color CleaningNotationColor
        {
            get => _cleaningNotationColor;
            set => SetProperty(value);
        }
    }
}