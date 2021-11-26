using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OpenCvSharp;
using YuzuMarker.BasicDataFormat;
using YuzuMarker.Common;

namespace YuzuMarker.DataFormat
{
    public class YuzuCleaningNotation : BasicYuzuNotation
    {
        private YuzuCleaningNotationType _cleaningNotationType;

        [Undoable]
        public YuzuCleaningNotationType CleaningNotationType
        {
            get => _cleaningNotationType;
            set => SetProperty(value);
        }

        public YuzuCleaningNotation(YuzuNotationGroup parentNotationGroup, YuzuCleaningNotationType type) : base(parentNotationGroup)
        {
            CleaningNotationType = type;
        }

        public UMat CleaningMask;

        public virtual void Load()
        {
            var tempMaskImagePath = Path.Combine(ParentNotationGroup.ParentImage.GetImageTempPath(), 
                "./" + ParentNotationGroup.Timestamp + "-cleaning-mask.png");
            if (!File.Exists(tempMaskImagePath))
            {
                var maskImagePath = Path.Combine(ParentNotationGroup.ParentImage.GetImageNotationPath(), 
                    "./" + ParentNotationGroup.Timestamp + "-cleaning-mask.png");
                if (!File.Exists(maskImagePath))
                {
                    using var src = new Mat(ParentNotationGroup.ParentImage.GetImageFilePath());
                    CleaningMask = new UMat(new Size(src.Cols, src.Rows), MatType.CV_8UC1, new Scalar(0));
                }
                else
                {
                    CleaningMask = new UMat();
                    using var src = Cv2.ImRead(maskImagePath, ImreadModes.Grayscale);
                    src.CopyTo(CleaningMask);
                }
            }
            else
            {
                CleaningMask = new UMat();
                using var src = Cv2.ImRead(tempMaskImagePath, ImreadModes.Grayscale);
                src.CopyTo(CleaningMask);
            }
        }

        public virtual void CustomLoad()
        {
        }

        public virtual void Write()
        {
            var tempMaskImagePath = Path.Combine(ParentNotationGroup.ParentImage.GetImageTempPath(), 
                "./" + ParentNotationGroup.Timestamp + "-cleaning-mask.png");
            var writeMat = CleaningMask.GetMat(AccessFlag.READ);
            Cv2.ImWrite(tempMaskImagePath, writeMat);
            writeMat.Dispose();
        }
        
        public virtual void CustomWrite()
        {
        }

        public virtual void Dispose()
        {
            CleaningMask.Dispose();
        }
        
        public virtual void Unload()
        {
            Write();
            Dispose();
        }
        
        public bool IsEmpty()
        {
            if (CleaningMask != null)
                if (!CleaningMask.IsDisposed)
                    if (CleaningMask.CvPtr != IntPtr.Zero)
                        return Cv2.CountNonZero(CleaningMask) == 0;
            return true;
        }

        public static void ConvertTo(ref YuzuCleaningNotation notation, YuzuCleaningNotationType type)
        {
            YuzuCleaningNotation newNotation;
            switch (type)
            {
                case YuzuCleaningNotationType.Color:
                    newNotation = new YuzuColorCleaningNotation(notation.ParentNotationGroup as YuzuNotationGroup);
                    break;
                case YuzuCleaningNotationType.Impainting:
                    newNotation = new YuzuImpaintingCleaningNotation(notation.ParentNotationGroup as YuzuNotationGroup);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            newNotation.CleaningMask = notation.CleaningMask.Clone();
            newNotation.CustomLoad();
            notation = newNotation;
        }
    }
}
