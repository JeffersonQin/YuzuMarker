using System.IO;
using OpenCvSharp;

namespace YuzuMarker.DataFormat
{
    public class YuzuImpaintingCleaningNotation : YuzuCleaningNotation
    {
        public YuzuImpaintingCleaningNotation(YuzuNotationGroup parentNotationGroup) : 
            base(parentNotationGroup, YuzuCleaningNotationType.Impainting) {}

        public UMat ImpaintingImage;
        
        public override void Load()
        {
            base.Load();
            CustomLoad();
        }

        public override void CustomLoad()
        {
            var tempImpaintingImagePath = Path.Combine(ParentNotationGroup.ParentImage.GetImageTempPath(), 
                "./" + ParentNotationGroup.Timestamp + "-impainting.png");
            if (!File.Exists(tempImpaintingImagePath))
            {
                var impaintingImagePath = Path.Combine(ParentNotationGroup.ParentImage.GetImageNotationPath(), 
                    "./" + ParentNotationGroup.Timestamp + "-impainting.png");
                if (!File.Exists(impaintingImagePath))
                {
                    using var src = new Mat(ParentNotationGroup.ParentImage.GetImageFilePath());
                    ImpaintingImage = new UMat(new Size(src.Cols, src.Rows), MatType.CV_8UC3, new Scalar(0, 0, 0));
                }
                else
                {
                    ImpaintingImage = new UMat();
                    using var src = Cv2.ImRead(impaintingImagePath);
                    src.CopyTo(ImpaintingImage);
                }
            }
            else
            {
                ImpaintingImage = new UMat();
                using var src = Cv2.ImRead(tempImpaintingImagePath);
                src.CopyTo(ImpaintingImage);
            }
        }

        public override void Write()
        {
            base.Write();
            CustomWrite();
        }

        public override void CustomWrite()
        {
            var tempImpaintingImagePath = Path.Combine(ParentNotationGroup.ParentImage.GetImageTempPath(), 
                "./" + ParentNotationGroup.Timestamp + "-impainting.png");
            var writeMat = ImpaintingImage.GetMat(AccessFlag.READ);
            Cv2.ImWrite(tempImpaintingImagePath, writeMat);
            writeMat.Dispose();
        }

        public override void Dispose()
        {
            ImpaintingImage.Dispose();
            base.Dispose();
        }

        public override void Unload()
        {
            Write();
            Dispose();
        }
    }
}