using System.IO;
using OpenCvSharp;
using YuzuMarker.Common;
using YuzuMarker.Utils;

namespace YuzuMarker.DataFormat
{
    [AncestorNotifiableMarker("ParentNotationGroup")]
    public class YuzuImpaintingCleaningNotation : YuzuCleaningNotation
    {
        private UMat _impaintingImage;
        
        [ChainNotifiable]
        [Undoable]
        public UMat ImpaintingImage
        {
            get => _impaintingImage;
            set => SetProperty(value, disposeAction: o => ((UMat)o).SafeDispose());
        }
        
        public YuzuImpaintingCleaningNotation(YuzuNotationGroup parentNotationGroup) : 
            base(parentNotationGroup, YuzuCleaningNotationType.Impainting) {}

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
            writeMat.SafeDispose();
        }

        public override void Dispose()
        {
            ImpaintingImage.SafeDispose();
            base.Dispose();
        }

        public override void Unload()
        {
            Write();
            Dispose();
        }
    }
}