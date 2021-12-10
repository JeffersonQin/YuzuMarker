using System.IO;
using OpenCvSharp;
using YuzuMarker.Common;
using YuzuMarker.Utils;

namespace YuzuMarker.DataFormat
{
    [AncestorNotifiableMarker("ParentNotationGroup")]
    public class YuzuInpaintingCleaningNotation : YuzuCleaningNotation
    {
        private UMat _inpaintingImage;
        
        [ChainNotifiable]
        [Undoable]
        public UMat InpaintingImage
        {
            get => _inpaintingImage;
            set => SetProperty(value, disposeAction: o => ((UMat)o).SafeDispose());
        }
        
        public YuzuInpaintingCleaningNotation(YuzuNotationGroup parentNotationGroup) : 
            base(parentNotationGroup, YuzuCleaningNotationType.Inpainting) {}

        public override void Load()
        {
            base.Load();
            CustomLoad();
        }

        public override void CustomLoad()
        {
            var tempInpaintingImagePath = Path.Combine(ParentNotationGroup.ParentImage.GetImageTempPath(), 
                "./" + ParentNotationGroup.Timestamp + "-inpainting.png");
            if (!File.Exists(tempInpaintingImagePath))
            {
                var inpaintingImagePath = Path.Combine(ParentNotationGroup.ParentImage.GetImageNotationPath(), 
                    "./" + ParentNotationGroup.Timestamp + "-inpainting.png");
                if (!File.Exists(inpaintingImagePath))
                {
                    using var src = new Mat(ParentNotationGroup.ParentImage.GetImageFilePath());
                    InpaintingImage = new UMat(new Size(src.Cols, src.Rows), MatType.CV_8UC3, new Scalar(0, 0, 0));
                }
                else
                {
                    InpaintingImage = new UMat();
                    using var src = Cv2.ImRead(inpaintingImagePath);
                    src.CopyTo(InpaintingImage);
                }
            }
            else
            {
                InpaintingImage = new UMat();
                using var src = Cv2.ImRead(tempInpaintingImagePath);
                src.CopyTo(InpaintingImage);
            }
        }

        public override void Write()
        {
            base.Write();
            CustomWrite();
        }

        public override void CustomWrite()
        {
            var tempInpaintingImagePath = Path.Combine(ParentNotationGroup.ParentImage.GetImageTempPath(), 
                "./" + ParentNotationGroup.Timestamp + "-inpainting.png");
            var writeMat = InpaintingImage.GetMat(AccessFlag.READ);
            Cv2.ImWrite(tempInpaintingImagePath, writeMat);
            writeMat.SafeDispose();
        }

        public override void Dispose()
        {
            InpaintingImage.SafeDispose();
            base.Dispose();
        }

        public override void Unload()
        {
            Write();
            Dispose();
        }
    }
}