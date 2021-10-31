using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OpenCvSharp;
using YuzuMarker.BasicDataFormat;

namespace YuzuMarker.DataFormat
{
    public class YuzuNotationGroup : BasicYuzuNotationGroup
    {
        public ResourcesTracker NotationResourcesTracker;
        
        public YuzuCleaningNotation CleaningNotation { get; set; }

        public YuzuNotationGroup(YuzuImage parentImage, int x, int y, string text, bool finished) : base(parentImage, x, y, text, finished)
        {
            CleaningNotation = new YuzuCleaningNotation(YuzuCleaningNotationType.Normal);
        }

        public YuzuNotationGroup(YuzuImage parentImage, long timestamp, int x, int y, string text, bool finished) : base(parentImage, timestamp, x, y, text, finished)
        {
            CleaningNotation = new YuzuCleaningNotation(YuzuCleaningNotationType.Normal);
        }
        
        public void LoadNotationResource()
        {
            NotationResourcesTracker = new ResourcesTracker();
            var tempMaskImagePath = Path.Combine(ParentImage.GetImageTempPath(), "./" + Timestamp + "-cleaning-mask.png");
            if (!File.Exists(tempMaskImagePath))
            {
                var maskImagePath = Path.Combine(ParentImage.GetImageNotationPath(), "./" + Timestamp + "-cleaning-mask.png");
                if (!File.Exists(maskImagePath))
                {
                    using var src = new Mat(ParentImage.GetImageFilePath());
                    CleaningNotation.CleaningMask = NotationResourcesTracker.T<UMat>(new UMat(new Size(src.Cols, src.Rows),
                        MatType.CV_8UC1, new Scalar(0)));
                }
                else
                {
                    CleaningNotation.CleaningMask = NotationResourcesTracker.T<UMat>(new UMat());
                    using var src = Cv2.ImRead(maskImagePath, ImreadModes.Grayscale);
                    src.CopyTo(CleaningNotation.CleaningMask);
                }
            }
            else
            {
                CleaningNotation.CleaningMask = NotationResourcesTracker.T<UMat>(new UMat());
                using var src = Cv2.ImRead(tempMaskImagePath, ImreadModes.Grayscale);
                src.CopyTo(CleaningNotation.CleaningMask);
            }
        }

        public void WriteNotationResource()
        {
            var tempMaskImagePath = Path.Combine(ParentImage.GetImageTempPath(), "./" + Timestamp + "-cleaning-mask.png");
            var writeMat = CleaningNotation.CleaningMask.GetMat(AccessFlag.READ);
            Cv2.ImWrite(tempMaskImagePath, writeMat);
            writeMat.Dispose();
        }

        public void UnloadNotationResource()
        {
            WriteNotationResource();
            CleaningNotation.CleaningMask.Dispose();
            NotationResourcesTracker.Dispose();
        }

        // Other kinds of notations
    }
}
