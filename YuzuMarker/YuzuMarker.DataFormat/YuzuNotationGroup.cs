using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OpenCvSharp;
using YuzuMarker.BasicDataFormat;
using YuzuMarker.Common;

namespace YuzuMarker.DataFormat
{
    public class YuzuNotationGroup : BasicYuzuNotationGroup
    {
        private YuzuCleaningNotation _cleaningNotation;

        [Undoable]
        public YuzuCleaningNotation CleaningNotation
        {
            get => _cleaningNotation;
            set => SetProperty(value, disposeAction: o => ((YuzuCleaningNotation)o).Dispose());
        }
        
        // Other kinds of notations
        
        public YuzuNotationGroup(YuzuImage parentImage, int x, int y, string text, bool finished) : base(parentImage, x, y, text, finished)
        {
            CleaningNotation = new YuzuColorCleaningNotation(this);
        }

        public YuzuNotationGroup(YuzuImage parentImage, long timestamp, int x, int y, string text, bool finished) : base(parentImage, timestamp, x, y, text, finished)
        {
            CleaningNotation = new YuzuColorCleaningNotation(this);
        }
        
        public void Load()
        {
            CleaningNotation.Load();
        }

        public void Write()
        {
            CleaningNotation.Write();
        }

        public void Dispose()
        {
            CleaningNotation.Dispose();
        }

        public void Unload()
        {
            CleaningNotation.Unload();
        }
    }
}
