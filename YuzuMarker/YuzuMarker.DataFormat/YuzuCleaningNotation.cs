using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using OpenCvSharp;
using YuzuMarker.Common;

namespace YuzuMarker.DataFormat
{
    public class YuzuCleaningNotation : NotifyObject
    {
        private YuzuCleaningNotationType _cleaningNotationType;

        public YuzuCleaningNotationType CleaningNotationType
        {
            get => _cleaningNotationType;
            set => SetProperty(ref _cleaningNotationType, value, beforeChanged: () =>
            {
                UndoRedoManager.PushRecord(CleaningNotationType, (o) =>
                {
                    CleaningNotationType = (YuzuCleaningNotationType)o;
                }, () => CleaningNotationType);
            });
        }

        public UMat CleaningMask;

        public YuzuCleaningNotation(YuzuCleaningNotationType type)
        {
            CleaningNotationType = type;
        }
        
        public bool IsEmpty()
        {
            if (CleaningMask != null)
                if (!CleaningMask.IsDisposed)
                    if (CleaningMask.CvPtr != IntPtr.Zero)
                        return Cv2.CountNonZero(CleaningMask) == 0;
            return true;
        }
    }
}
